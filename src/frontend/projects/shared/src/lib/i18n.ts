export type Lang = 'en' | 'ar';

export const EN = {
  product: 'Customer Support CRM',
  agent: 'Agent',
  portal: 'Portal',
  admin: 'Admin',
  knowledge: 'Knowledge',
  signIn: 'Sign in',
  signOut: 'Sign out',
  email: 'Email',
  password: 'Password',
  languageToggle: 'العربية',
  homeLead: 'Open the agent workspace through the gateway. Demo user: agent@crm.local / Crm!123',
  workspaceReady: 'Agent workspace is ready',
  workspaceHint: 'Customers and tickets land here in later SDD slices.',
  comingSoon: 'Coming soon',
  bootstrap: 'Customers bootstrap',
  signedInAs: 'Signed in as',
} as const;

export const AR = {
  product: 'نظام دعم العملاء',
  agent: 'الوكيل',
  portal: 'البوابة',
  admin: 'الإدارة',
  knowledge: 'المعرفة',
  signIn: 'تسجيل الدخول',
  signOut: 'تسجيل الخروج',
  email: 'البريد الإلكتروني',
  password: 'كلمة المرور',
  languageToggle: 'English',
  homeLead: 'افتح مساحة الوكيل عبر البوابة. مستخدم التجربة: agent@crm.local / Crm!123',
  workspaceReady: 'مساحة عمل الوكيل جاهزة',
  workspaceHint: 'العملاء والتذاكر تصل إلى هنا في الشرائح التالية.',
  comingSoon: 'قريبًا',
  bootstrap: 'حالة خدمة العملاء',
  signedInAs: 'مسجل الدخول باسم',
} as const;

export type MessageKey = keyof typeof EN;
