import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';

export function providePortalCore(): EnvironmentProviders {
  return makeEnvironmentProviders([provideHttpClient()]);
}
