# Sympli-SEO

## Introduction
This project is written for Sympli company to show case technical, design and logic skills.

## The goal
Creating an application that counts the number of a URL returned in the search results from different search enginges using certain keywords.

## Architecture
The process' flow would start from front-end. Here use enters some keywords just like using any search engine. In addition he would provide the URL which he is intersted in its performance. Then front-end would send all of those to the backend. Backend would first try to find the analysed results in the cache and serve the front-end by returning the cached data. Otherwise, it would call the search-engine and get the results and copy them into the cach and return them to the front-end.

## Design
The main players in this design are SearchResultProvider and SearchResultService. The former send the request to a search engine and get the response while the later analyse the response. Currently, there are only one 2 implimentation of the former (SearchEngineProvider) for Google and Bing but other search engines could be implemented.

The other player is the repository which stores all the results in the database. The caching is based on the latest stored result in the database whether it's less than an hour old or not.

## Technology stack
- The front-end is using React and Redux. 
- The back-end is using ASP.Net Core 3.0 and Entity Frame work 3.0. 
- The database is SQLite.

## User interface
### Main search page
![screenshot - 1](https://github.com/mkokabi/Sympli-SEO/blob/master/img/Screenshot-01.png "Screenshot - 1")

### Get search restuls page
![screenshot - 2](https://github.com/mkokabi/Sympli-SEO/blob/master/img/Screenshot-02.png "Screenshot - 2")


## Assumptions
- Google putting the found url in a tag like:
```html
<div class=""BNeawe UPmit AP7Wnd"">{url}</div>
```
- Some of the URLs would have a trailing part which is after ```&#8250;``` 
