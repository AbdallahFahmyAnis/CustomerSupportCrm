import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';

export function provideAgentCore(): EnvironmentProviders {
  return makeEnvironmentProviders([provideHttpClient()]);
}
