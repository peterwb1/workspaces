// Karma configuration file, see link for more information
// https://karma-runner.github.io/1.0/config/configuration-file.html

var webpackConfig = require('./webpack.test');

module.exports = function (config) {
  var _config = {
    basePath: '',

    frameworks: ['jasmine'],

    files: [
      {pattern: './config/karma-test-shim.js', watched: false}
    ],

    preprocessors: {
      './config/karma-test-shim.js': ['webpack', 'sourcemap']
    },

    webpack: webpackConfig,

    webpackMiddleware: {
      stats: 'errors-only'
    },

    webpackServer: {
      noInfo: true
    },

    reporters: ['progress', 'kjhtml'],
    port: 9876,
    colors: true,
    logLevel: config.LOG_INFO,
    autoWatch: false,
    browsers: ['Chrome'],
    singleRun: true
  };

  config.set(_config);
};


// module.exports = function (config) {
//   config.set({
//     basePath: '',
//     frameworks: ['jasmine', '@angular-devkit/build-angular'],
//     plugins: [
//       require('karma-jasmine'),
//       require('karma-chrome-launcher'),
//       require('karma-jasmine-html-reporter'),
//       require('karma-coverage'),
//       require('@angular-devkit/build-angular/plugins/karma')
//     ],
//     client: {
//       jasmine: {
//         // you can add configuration options for Jasmine here
//         // the possible options are listed at https://jasmine.github.io/api/edge/Configuration.html
//         // for example, you can disable the random execution with `random: false`
//         // or set a specific seed with `seed: 4321`
//       },
//       clearContext: false // leave Jasmine Spec Runner output visible in browser
//     },
//     jasmineHtmlReporter: {
//       suppressAll: true // removes the duplicated traces
//     },
//     coverageReporter: {
//       dir: require('path').join(__dirname, './coverage/lawyer-experience-two'),
//       subdir: '.',
//       reporters: [
//         { type: 'html' },
//         { type: 'text-summary' }
//       ]
//     },
//     reporters: ['progress', 'kjhtml'],
//     port: 9876,
//     colors: true,
//     logLevel: config.LOG_INFO,
//     autoWatch: true,
//     browsers: ['Chrome'],
//     singleRun: false,
//     restartOnFileChange: true
//   });
// };
