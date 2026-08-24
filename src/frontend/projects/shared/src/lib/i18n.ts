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
  homeLead: 'Open via gateway. Demo: agent@crm.local or admin@crm.local / Crm!123',
  workspaceReady: 'Agent workspace is ready',
  workspaceHint: 'Customers and tickets land here in later SDD slices.',
  comingSoon: 'Coming soon',
  bootstrap: 'Customers bootstrap',
  signedInAs: 'Signed in as',
  customers: 'Customers',
  tickets: 'Tickets',
  users: 'Users',
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
  homeLead: 'افتح عبر البوابة. تجربة: agent@crm.local أو admin@crm.local / Crm!123',
  workspaceReady: 'مساحة عمل الوكيل جاهزة',
  workspaceHint: 'العملاء والتذاكر تصل إلى هنا في الشرائح التالية.',
  comingSoon: 'قريبًا',
  bootstrap: 'حالة خدمة العملاء',
  signedInAs: 'مسجل الدخول باسم',
  customers: 'العملاء',
  tickets: 'التذاكر',
  users: 'المستخدمون',
} as const;

export type MessageKey = keyof typeof EN;
