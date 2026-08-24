import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';

/** Shell core providers (extend with interceptors as needed). */
export function provideShellCore(): EnvironmentProviders {
  return makeEnvironmentProviders([provideHttpClient()]);
}
