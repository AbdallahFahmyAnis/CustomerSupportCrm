import {
  canAccessAdmin,
  canAccessAgentWorkspace,
  canAccessCustomerPortal,
  homePathForRole,
  isCustomerRole,
} from './roles';

describe('roles', () => {
  describe('homePathForRole', () => {
    it('routes Admin to /admin', () => {
      expect(homePathForRole('Admin')).toBe('/admin');
    });

    it('routes Customer to /portal', () => {
      expect(homePathForRole('Customer')).toBe('/portal');
    });

    it('routes Agent to /agent', () => {
      expect(homePathForRole('Agent')).toBe('/agent');
    });

    it('routes Lead to /agent', () => {
      expect(homePathForRole('Lead')).toBe('/agent');
    });
  });

  describe('canAccessAdmin', () => {
    it('allows Admin only', () => {
      expect(canAccessAdmin('Admin')).toBeTrue();
      expect(canAccessAdmin('Agent')).toBeFalse();
      expect(canAccessAdmin('Customer')).toBeFalse();
    });
  });

  describe('canAccessAgentWorkspace', () => {
    it('allows staff roles', () => {
      expect(canAccessAgentWorkspace('Admin')).toBeTrue();
      expect(canAccessAgentWorkspace('Lead')).toBeTrue();
      expect(canAccessAgentWorkspace('Agent')).toBeTrue();
      expect(canAccessAgentWorkspace('Customer')).toBeFalse();
    });
  });

  describe('canAccessCustomerPortal', () => {
    it('allows customers and staff', () => {
      expect(canAccessCustomerPortal('Customer')).toBeTrue();
      expect(canAccessCustomerPortal('Agent')).toBeTrue();
      expect(isCustomerRole('Customer')).toBeTrue();
    });
  });
});
