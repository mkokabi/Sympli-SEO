# Sympli-SEO

## Introduction
This project is written for Sympli company to show case technical, design and logic skills.

## The goal
Creating an application that counts the number of a URL returned in the search results from different search enginges using certain keywords.

## Architecture
The process' flow would start from front-end. Here use enters some keywords like search engines but in addition he would provide the URL which he is intersted in their performance. Then front-end would send all of those to the backend. Backend would first try to find them in the cache and serve the front-end by returning the data. Otherwise, it would call the search-engine and get the results and copy them into the cach and return them to the front-end.

