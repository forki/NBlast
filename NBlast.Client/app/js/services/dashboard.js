define(['underscore', 'jquery', 'services/settings'], function(_, $, settings) {
	'use strict';
	var dashboard = {
		groupBy: function (field, limit) {
			var resourcesPart = [field, '/', parseInt(limit, 10) || 10].join(''),
				url = settings.appendToBackendUrl('dashboard/group-by/' + resourcesPart);
			return $.getJSON(url);
		}
	};
	return settings.isTestEnv() ? dashboard : Object.freeze(dashboard);
});
