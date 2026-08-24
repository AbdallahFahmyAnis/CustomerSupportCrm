import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';

export function provideAdminCore(): EnvironmentProviders {
  return makeEnvironmentProviders([provideHttpClient()]);
}
