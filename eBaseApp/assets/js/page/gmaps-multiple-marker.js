"use strict";

// initialize map
var map = new GMaps({
  div: '#map',
    lat: -26.183498,
    lng: 27.9667321,
  zoom: 8
});
// Added markers to the map
map.addMarker({
    lat: -26.183498,
    lng: 27.9667321,
  title: 'Airport',
  infoWindow: {
    content: '<h6>Airport</h6><p>Sardar Vallabhbhai Patel International Airport, <br>Ahmedabad</p><p><a target="_blank" href="http://example.com">Website</a></p>'
  }
});
map.addMarker({
    lat: -26.1762985,
    lng: 27.6725043,
  title: 'Bus Stop',
  infoWindow: {
    content: '<h6>Bus Stop</h6><p>Rajkot GSRTC Bus Stop</p><p><a target="_blank" href="https://example.com/">Website</a></p>'
  }
});

map.addMarker({
    lat: -26.3017468,
    lng: 28.1868497,
  title: 'University',
  infoWindow: {
    content: '<h6>University</h6><p>M S Uni Head Office, Officers Railway Colony, Pratapgunj, Vadodara, Gujarat 390002, India</p><p><a target="_blank" href="http://example.com/">Website</a></p>'
  }
});
