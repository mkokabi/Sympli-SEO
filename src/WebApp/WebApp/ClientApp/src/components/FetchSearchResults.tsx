import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';
import { ApplicationState } from '../store';
import * as SearchResultstsStore from '../store/SearchResults';

// At runtime, Redux will merge together...
type SearchResultsProps =
  SearchResultstsStore.SearchResultsState // ... state we've requested from the Redux store
  & typeof SearchResultstsStore.actionCreators // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters


class FetchSearchResults extends React.PureComponent<SearchResultsProps> {
  // This method is called when the component is first added to the document
  public componentDidMount() {
    this.ensureDataFetched();
  }

  // This method is called when the route parameters change
  public componentDidUpdate() {
    this.ensureDataFetched();
  }

  public render() {
    return (
      <React.Fragment>
        <h1 id="tabelLabel">Search results</h1>
        {this.renderSearchResultsTable()}
        {this.renderPagination()}
      </React.Fragment>
    );
  }

  private ensureDataFetched() {
    const startDateIndex = parseInt(this.props.match.params.startDateIndex, 10) || 0;
    this.props.requestSearchResults(startDateIndex);
  }

  private renderSearchResultsTable() {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Date</th>
            <th>Temp. (C)</th>
            <th>Temp. (F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {this.props.searchResults.map((searchResult: SearchResultstsStore.SearchResult) =>
            <tr key={searchResult.date}>
              <td>{searchResult.date}</td>
              <td>{searchResult.temperatureC}</td>
              <td>{searchResult.temperatureF}</td>
              <td>{searchResult.summary}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  private renderPagination() {
    const prevStartDateIndex = (this.props.startDateIndex || 0) - 5;
    const nextStartDateIndex = (this.props.startDateIndex || 0) + 5;

    return (
      <div className="d-flex justify-content-between">
        {prevStartDateIndex >= 0 ?
          <Link className='btn btn-sm btn-info' to={`/fetch-data/${prevStartDateIndex}`}>Previous</Link>
          : <button className='btn btn-light btn-sm' disabled>Previous</button>}
        {this.props.isLoading && <span>Loading...</span>}
        <Link className='btn btn-info btn-sm' to={`/fetch-data/${nextStartDateIndex}`}>Next</Link>
      </div>
    );
  }
}

export default connect(
  (state: ApplicationState) => state.searchResults, // Selects which state properties are merged into the component's props
  SearchResultstsStore.actionCreators // Selects which action creators are merged into the component's props
)(FetchSearchResults as any);
