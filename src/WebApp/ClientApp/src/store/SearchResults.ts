import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface SearchResultsState {
  isLoading: boolean;
  startDateIndex?: number;
  searchResults: SearchResult[];
  isSearching: boolean;
  searchResult: SearchResult;
  searchParam: SearchParams;
}

export interface SearchResult {
  date: string;
  url: string;
  keywords: string[];
  results: number[];
}

export interface SearchParams {
  url: string;
  keywords: string[];
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestSearchResultsAction {
  type: 'REQUEST_SEARCH_RESULTS';
  startDateIndex: number;
}

interface ReceiveSearchResultsAction {
  type: 'RECEIVE_SEARCH_RESULTS';
  startDateIndex: number;
  searchResults: SearchResult[];
}

interface StartingSearchAction {
  type: 'STARTING_SEARCH';
  searchParams: SearchParams;
}

interface SearchCompletedAction {
  type: 'SEARCH_COMPLETED',
  searchResult: SearchResult;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestSearchResultsAction | ReceiveSearchResultsAction | StartingSearchAction | SearchCompletedAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
  requestSearchResults: (startDateIndex: number): AppThunkAction<KnownAction> => (dispatch, getState) => {
    // Only load data if it's something we don't already have (and are not already loading)
    const appState = getState();
    if (appState && appState.searchResults && startDateIndex !== appState.searchResults.startDateIndex) {
      fetch(`/api/SearchResults`)
        .then(response => response.json() as Promise<SearchResult[]>)
        .then(data => {
          dispatch({ type: 'RECEIVE_SEARCH_RESULTS', startDateIndex: startDateIndex, searchResults: data });
        });

      dispatch({ type: 'REQUEST_SEARCH_RESULTS', startDateIndex: startDateIndex });
    }
  }
};

export const searchActionCreators = {
  postSearchRequest: (searchParams: SearchParams): AppThunkAction<KnownAction> => (dispatch, getState) => {
    const appState = getState();
    if (appState) {
      fetch(`/api/SearchResults`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
          // 'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: JSON.stringify({ url: searchParams.url, keywords: searchParams.keywords })
      })
        .then(response => response.json() as Promise<SearchResult>)
        .then(data => {
          dispatch({ type: 'SEARCH_COMPLETED', searchResult: data });
        });

      dispatch({ type: 'STARTING_SEARCH', searchParams: searchParams});
    }
  }
}

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: SearchResultsState = {
  searchResults: [], isLoading: false, isSearching: false,
  searchResult: { date: "", keywords: [], url: "", results: [] },
  searchParam: { url: "", keywords: [] }
};

export const reducer: Reducer<SearchResultsState> = (state: SearchResultsState | undefined, incomingAction: Action): SearchResultsState => {
  if (state === undefined) {
    return unloadedState;
  }

  const action = incomingAction as KnownAction;
  switch (action.type) {
    case 'REQUEST_SEARCH_RESULTS':
      return {
        startDateIndex: action.startDateIndex,
        searchResults: state.searchResults,
        isLoading: true,
        isSearching: false,
        searchResult: state.searchResult,
        searchParam: state.searchParam
      };
    case 'RECEIVE_SEARCH_RESULTS':
      // Only accept the incoming data if it matches the most recent request. This ensures we correctly
      // handle out-of-order responses.
      if (action.startDateIndex === state.startDateIndex) {
        return {
          startDateIndex: action.startDateIndex,
          searchResults: action.searchResults,
          isLoading: false,
          isSearching: false,
          searchResult: state.searchResult,
          searchParam: state.searchParam
        };
      }
      break;
    case 'SEARCH_COMPLETED':
      return {
        startDateIndex: undefined,
        searchResults: [],
        isLoading: false,
        isSearching: false,
        searchResult: action.searchResult,
        searchParam: state.searchParam
      };
  }

  return state;
};
