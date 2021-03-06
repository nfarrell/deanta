﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

//some default pre init
var Countly = Countly || {};
Countly.q = Countly.q || [];

//provide countly initialization parameters
Countly.app_key = '6488cf3ed5afe7c1b09f0741e491e70006af1418';
Countly.url = 'https://stats.boafy.com/';

Countly.q.push(['track_sessions']);
Countly.q.push(['track_pageview']);
Countly.q.push(['track_clicks']);
Countly.q.push(['track_scrolls']);
Countly.q.push(['track_errors']);
Countly.q.push(['track_links']);
Countly.q.push(['track_forms']);
Countly.q.push(['collect_from_forms']);

//load countly script asynchronously
(function () {
    var cly = document.createElement('script'); cly.type = 'text/javascript';
    cly.async = true;
    //enter url of script here
    cly.src = 'https://cdnjs.cloudflare.com/ajax/libs/countly-sdk-web/18.8.2/countly.min.js';
    cly.onload = function () { Countly.init() };
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(cly, s);
})();