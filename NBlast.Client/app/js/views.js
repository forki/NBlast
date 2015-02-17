(function() {
	'use strict';
	var views = {
		getSearch: function() {
			return require('../views/search.html');
		},
		getDashboard: function() {
			return require('../views/dashboard.html');
		},
		getDetails: function() {
			return require('../views/details.html');
		}
	};

	module.exports = views;
})();