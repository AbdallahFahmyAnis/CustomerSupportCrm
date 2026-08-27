const { withNativeFederation, shareAll } = require('@angular-architects/native-federation/config');

module.exports = withNativeFederation({
  name: 'admin-mfe',
  exposes: {
    './Routes': './projects/admin-mfe/src/app/remote.routes.ts',
  },
  shared: {
    ...shareAll({ singleton: true, strictVersion: true, requiredVersion: 'auto' }),
  },
  skip: [
    'rxjs/ajax',
    'rxjs/fetch',
    'rxjs/testing',
    'rxjs/webSocket',
    'apexcharts',
    'ng-apexcharts',
  ],
  features: {
    ignoreUnusedDeps: true,
  },
});
