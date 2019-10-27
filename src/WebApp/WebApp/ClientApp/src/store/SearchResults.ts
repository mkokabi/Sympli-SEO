import { Action, Reducer } from 'redux';
import { AppThunkAction } from '.';

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface SearchResultsState {
  isLoading: boolean;
  startDateIndex?: number;
  searchResults: SearchResult[];
}

export interface SearchResult {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
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

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestSearchResultsAction | ReceiveSearchResultsAction;

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

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: SearchResultsState = { searchResults: [], isLoading: false };

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
        isLoading: true
      };
    case 'RECEIVE_SEARCH_RESULTS':
      // Only accept the incoming data if it matches the most recent request. This ensures we correctly
      // handle out-of-order responses.
      if (action.startDateIndex === state.startDateIndex) {
        return {
          startDateIndex: action.startDateIndex,
          searchResults: action.searchResults,
          isLoading: false
        };
      }
      break;
  }

  return state;
};
