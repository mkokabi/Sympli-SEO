import * as React from 'react';
import { connect } from 'react-redux';
import Button from 'reactstrap/lib/Button';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as SearchResultstsStore from '../store/SearchResults';

// At runtime, Redux will merge together...
type SearchProps =
  SearchResultstsStore.SearchResultsState // ... state we've requested from the Redux store
  & typeof SearchResultstsStore.searchActionCreators // ... plus action creators we've requested
  & RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters


class Search extends React.PureComponent<SearchProps> {
  constructor(props: any) {
    super(props);
    this.state = { url: '', keywords: '' };

    this.onSearchClick = this.onSearchClick.bind(this);
    this.handleInputChange = this.handleInputChange.bind(this);
  }

  handleInputChange(event: any) {
    const target = event.target;
    const value = target.value;
    const name = target.name;

    if (name === "url") {
      this.props.searchParam.url = value;
    } else if (name === "keywords") {
      this.props.searchParam.keywords = value.split(',');
    }
  }

  onSearchClick(event: any) {
    this.props.postSearchRequest({ url: this.props.searchParam.url, keywords: this.props.searchParam.keywords });
  }

  public render() {
    return (
      <React.Fragment>
        <div className="form-group">
          <label>Enter the URL you are interested:</label>
          <input type="text" name="url"
            onChange={this.handleInputChange}
          />
        </div>

        <div className="form-group">
          <label>Enter the keywords:</label>
          <input type="text" name="keywords"
            onChange={this.handleInputChange}
          />
        </div>

        <Button type="button" className="btn btn-success" onClick={this.onSearchClick}>Search</Button>
        {this.renderSearchResultsTable()}

      </React.Fragment>
    );
  }

  private renderSearchResultsTable() {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Date</th>
            <th>URL</th>
            <th>Kewords</th>
            <th>Results</th>
          </tr>
        </thead>
        <tbody>
          <tr key={this.props.searchResult.date}>
            <td>{this.props.searchResult.date}</td>
            <td>{this.props.searchResult.url}</td>
            <td>{this.props.searchResult.keywords.join("+")}</td>
            <td>{this.props.searchResult.results.length}:[{this.props.searchResult.results.join(",")}]</td>
            </tr>
        </tbody>
      </table>
    );
  }
}

export default connect(
  (state: ApplicationState) => state.searchResults, // Selects which state properties are merged into the component's props
  SearchResultstsStore.searchActionCreators // Selects which action creators are merged into the component's props
)(Search as any);
