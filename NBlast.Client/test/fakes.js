(function () {
	'use strict';
	var config = require('../app/js/config'),
		views = require('../app/js/views'),
		sinon = require('sinon'),
		mocker = null,
		$ = function() {},
		nothing, fakes, amplify, sammy;

	nothing = function () {
		return '';
	};
	//noinspection JSUnusedGlobalSymbols
	amplify = {
		store: function () {
			return '';
		}
	};
	$.trim = function (str) {
		return [str].join('').trim();
	};
	$.getJSON = nothing;
	$.ajax = nothing;

	sammy = {
		setLocation: nothing,
		getLocation: nothing,
		runRoute: nothing
	};
	fakes = {
		mocker: function() {
			return mocker;
		},
		amplify: function () {
			return amplify;
		},
		jquery: function () {
			return $;
		},
		sammy: function () {
			return sammy;
		}
	};

	beforeEach(function() { // jshint ignore:line
		mocker = sinon.sandbox.create();
		mocker.stub(config, 'sammy', function() {
			return fakes.sammy;
		});
		mocker.stub(config, 'amplify', fakes.amplify);
		mocker.stub(config, 'jquery', fakes.jquery);
		mocker.stub(views, 'getSearch', nothing);
		mocker.stub(views, 'getDashboard', nothing);
		mocker.stub(views, 'getDetails', nothing);
		mocker.stub(views, 'getNotification', nothing);
		mocker.stub(views, 'getQueue', nothing);
	});

	afterEach(function(){ // jshint ignore:line
		if (fakes.mocker()) {
			fakes.mocker().restore();
		}
	});

	module.exports = fakes;
})();