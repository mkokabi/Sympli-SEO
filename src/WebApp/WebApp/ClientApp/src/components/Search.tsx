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

    if (name == "url") {
      this.props.searchParam.url = value;
    } else if (name == "keywords") {
      this.props.searchParam.keywords = value.split(',');
    }
    this.setState({
      [name]: value
    });
  }

  onSearchClick(event: any) {
    this.props.postSearchRequest({ url: this.props.searchParam.url, keywords: this.props.searchParam.keywords });
  }

  public render() {
    return (
      <React.Fragment>
        <label>Enter the URL you are interested:</label>
        <input type="text" name="url"
          onChange={this.handleInputChange}
        />

        <label>Enter the keywords:</label>
        <input type="text" name="keywords"
          onChange={this.handleInputChange}
        />

        <Button type="button" className="btn btn-success" onClick={this.onSearchClick}>Search</Button>
      </React.Fragment>
    );
  }
}

export default connect(
  (state: ApplicationState) => state.searchResults, // Selects which state properties are merged into the component's props
  SearchResultstsStore.searchActionCreators // Selects which action creators are merged into the component's props
)(Search as any);
