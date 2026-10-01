# 📘 Stock Exchange — Admin Frontend Authentication & Integration Guide
## دليل تكامل الواجهة الأمامية للوحة الإدارة (Admin Frontend) مع نظام المصادقة وإدارة الأخطاء

يقدم هذا المستند دليلاً شاملاً لكيفية ربط وتكامل الـ Frontend الخاص بلوحة تحكم الأدمن (Admin Dashboard) مع واجهات الـ Backend (ASP.NET Core Web API)، مع تغطية كاملة لعمليات:
1. **تسجيل الدخول (Login & Admin Role Verification)**
2. **نسيان كلمة المرور (Forget Password)**
3. **التحقق من كود الـ OTP (Verify OTP)**
4. **تعيين كلمة المرور الجديدة (Reset Password)**
5. **تجديد التوكن التلقائي (Refresh Token & Token Rotation)**
6. **تسجيل الخروج (Logout)**
7. **التوافق الكامل مع معالج الأخطاء العام (Global Exception Handling)** وكيفية التعامل مع `ApiResponse<T>` ورموز الأخطاء (400, 401, 403, 404, 500).

---

## 📑 جدول المحتويات (Table of Contents)
- [1. المبادئ الأساسية وإعدادات الـ API](#1-المبادئ-الأساسية-وإعدادات-الـ-api)
  - [Base URL & Headers](#base-url--headers)
  - [دعم اللغات (Localization)](#دعم-اللغات-localization)
- [2. هيكل الاستجابة الموحد والتعامل مع الأخطاء (Global Exception Handling)](#2-هيكل-الاستجابة-الموحد-والتعامل-مع-الأخطاء-global-exception-handling)
  - [نموذج `ApiResponse<T>`](#نموذج-apiresponset)
  - [أنواع الأخطاء ورموز الحالة (HTTP Status Codes)](#أنواع-الأخطاء-ورموز-الحالة-http-status-codes)
  - [دالة استخراج الأخطاء للـ Frontend Form Validation](#دالة-استخراج-الأخطاء-للـ-frontend-form-validation)
- [3. تفاصيل الـ Endpoints الخاصة بالمصادقة](#3-تفاصيل-الـ-endpoints-الخاصة-بالمصادقة)
  - [1. تسجيل الدخول — Login](#1-تسجيل-الدخول--login)
  - [2. نسيان كلمة المرور — Forget Password](#2-نسيان-كلمة-المرور--forget-password)
  - [3. التحقق من رمز الـ OTP — Verify OTP](#3-التحقق-من-رمز-الـ-otp--verify-otp)
  - [4. تعيين كلمة المرور الجديدة — Reset Password](#4-تعيين-كلمة-المرور-الجديدة--reset-password)
  - [5. تجديد التوكن — Refresh Token](#5-تجديد-التوكن--refresh-token)
  - [6. تسجيل الخروج — Logout](#6-تسجيل-الخروج--logout)
- [4. كود تنفيذي متكامل للـ Frontend (Axios + Interceptors + Auto-Refresh)](#4-كود-تنفيذي-متكامل-للـ-frontend-axios--interceptors--auto-refresh)
  - [تعريف الأنواع (TypeScript Interfaces)](#تعريف-الأنواع-typescript-interfaces)
  - [إعداد Axios مع Interceptor للتجديد التلقائي للتوكن (Token Rotation)](#إعداد-axios-مع-interceptor-للتجديد-التلقائي-للتوكن-token-rotation)
  - [ملف خدمات المصادقة (Auth Service)](#ملف-خدمات-المصادقة-auth-service)
- [5. إرشادات أمنية هامة للـ Admin Frontend](#5-إرشادات-أمنية-هامة-للـ-admin-frontend)

---

## 1. المبادئ الأساسية وإعدادات الـ API

### Base URL & Headers
- **Base Route:** `/api/v1/authentication`
- **مثال:** `https://api.yourdomain.com/api/v1/authentication`

يجب إرسال الـ Headers التالية في كل طلب:
| الـ Header | القيمة | الوصف |
| :--- | :--- | :--- |
| `Content-Type` | `application/json` | تنسيق البيانات المرسلة |
| `Accept` | `application/json` | تنسيق الاستجابة المطلوب |
| `Accept-Language` | `ar` أو `en` | لتحديد لغة رسائل الخطأ والنجاح القادمة من السيرفر |
| `Authorization` | `Bearer <AccessToken>` | مطلوب في الطلبات المحمية مثل `logout` |

### دعم اللغات (Localization)
النظام مزود بـ `CustomExceptionHandlerMiddleware` و `JsonLocalizationProvider`. لتحديد لغة الرسائل:
- أرسل Header: `Accept-Language: ar` أو `Accept-Language: en`.
- أو كـ Query Parameter: `?lang=ar` أو `?lang=en`.

---

## 2. هيكل الاستجابة الموحد والتعامل مع الأخطاء (Global Exception Handling)

جميع الـ Endpoints (سواء نجحت العملية أو فشلت) تعيد نفس الـ JSON Structure بأسلوب **camelCase**:

### نموذج `ApiResponse<T>`

```typescript
export interface ApiResponse<T = any> {
  success: boolean;            // true في حالة النجاح، false في حالة الخطأ
  isSuccess: boolean;          // مطابقة لـ success
  message: string | null;      // رسالة نصية مترجمة (نجاح أو ملخص الخطأ)
  statusCode: number;          // رمز HTTP (200, 201, 400, 401, 403, 404, 500)
  data: T | null;              // البيانات المرجعة في حالة النجاح
  errors: Record<string, string[]>; // قاموس يحتوي على أخطاء الـ Validation أو الأخطاء العامة
}
```

### أ) استجابة النجاح (Success Response Example):
```json
{
  "success": true,
  "isSuccess": true,
  "message": "Operation completed successfully.",
  "statusCode": 200,
  "data": { ... },
  "errors": {}
}
```

### ب) استجابة خطأ تحقق من المدخلات (Validation Error - 400 Bad Request):
عند إرسال بيانات غير صحيحة، يرجع الـ Backend مصفوفة أخطاء مجمعة حسب الحقل:
```json
{
  "success": false,
  "isSuccess": false,
  "message": "One or more validation errors occurred.",
  "statusCode": 400,
  "data": null,
  "errors": {
    "Email": [
      "Email is required."
    ],
    "Password": [
      "Password is required."
    ]
  }
}
```

### جـ) استجابة خطأ عام أو أمني (Business / Security Error):
عند حدوث خطأ عام غير مرتبط بحقل معين، ستجده دائماً تحت المفتاح `"General"`:
```json
{
  "success": false,
  "isSuccess": false,
  "message": "Invalid email or password.",
  "statusCode": 401,
  "data": null,
  "errors": {
    "General": [
      "Invalid email or password."
    ]
  }
}
```

---

### أنواع الأخطاء ورموز الحالة (HTTP Status Codes)

| Status Code | النوع في السيرفر | متى يحدث؟ | ما يجب على الواجهة الأمامية فعله؟ |
| :---: | :--- | :--- | :--- |
| **`400`** | `ValidationException` / `BadRequestException` | بيانات غير مكتملة، رمز التحقق منتهي أو غير صالح، كلمات المرور غير متطابقة. | عرض الأخطاء المحددة أسفل حقول الإدخال (`errors[field]`) أو عرض رسالة عامة عبر Toast. |
| **`401`** | `UnAuthorizedException` / `JwtBearer Challenge` | بيانات الدخول غير صحيحة، أو الـ Access Token منتهي الصلاحية (`SessionExpired`)، أو تم إبطال التوكن (`SessionRevoked`). | إذا كان الطلب محمياً والـ Token انتهى، يتم استدعاء `/refresh-token` تلقائياً. إذا فشل، يتم تحويل المستخدم لصفحة تسجيل الدخول. |
| **`403`** | `ForbiddenException` / `RoleAuthorize` | المستخدم حسابه محذوف/معطل، أو لا يملك رتبة `Admin` للدخول للوحة التحكم. | منع الدخول وعرض رسالة "غير مصرح لك بالوصول إلى لوحة التحكم"، ومسح أي بيانات محفوظة. |
| **`404`** | `NotFoundException` | الحساب غير موجود في النظام عند محاولة استرجاع كلمة المرور. | عرض رسالة "البريد الإلكتروني غير مسجل لدينا". |
| **`500 / 503`** | `ServerException` / `SocketException` | خطأ داخلي في الخادم أو تعذر الاتصال بسيرفر البريد (SMTP). | عرض رسالة خطأ عامة وودية دون إرباك المستخدم بتفاصيل السيرفر. |

---

### دالة استخراج الأخطاء للـ Frontend Form Validation

يمكنك استخدام هذه الدالة المساعدة في تطبيقات React, Vue, أو Angular لعرض رسائل الأخطاء القادمة من الـ API مباشرة:

```typescript
export function extractApiErrors(errorResponse: ApiResponse | any): {
  generalMessage: string;
  fieldErrors: Record<string, string>;
} {
  const data = errorResponse?.response?.data as ApiResponse | undefined;
  
  const generalMessage = data?.message || "حدث خطأ غير متوقع، يرجى المحاولة لاحقاً.";
  const fieldErrors: Record<string, string> = {};

  if (data?.errors) {
    for (const [key, messages] of Object.entries(data.errors)) {
      if (key.toLowerCase() !== "general" && messages && messages.length > 0) {
        // تحويل أول حرف إلى camelCase ليتطابق مع الـ Form Inputs (مثل Email -> email)
        const camelCaseKey = key.charAt(0).toLowerCase() + key.slice(1);
        fieldErrors[camelCaseKey] = messages[0];
      }
    }
  }

  return { generalMessage, fieldErrors };
}
```

---

## 3. تفاصيل الـ Endpoints الخاصة بالمصادقة

### 1. تسجيل الدخول — Login
- **Endpoint:** `POST /api/v1/authentication/login`
- **الوصف:** مصادقة المشرف والتحقق من رتبته (`Admin`) واستلام الـ Access Token والـ Refresh Token.

#### Request Body:
```json
{
  "email": "admin@example.com",
  "password": "SecurePassword123!"
}
```

#### Success Response (`200 OK`):
```json
{
  "success": true,
  "isSuccess": true,
  "message": "Success",
  "statusCode": 200,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "4a7b9c1d-8f2e-4b6a-9c3d-1e2f3a4b5c6d",
    "fullName": "System Admin",
    "email": "admin@example.com",
    "phoneNumber": "+201234567890",
    "roles": "Admin",
    "id": "c1f7a012-6831-419a-a3a2-1cb2b1928abc",
    "profilePictureUrl": "https://cdn.example.com/profiles/admin.png"
  },
  "errors": {}
}
```

> [!IMPORTANT]
> **التحقق من رتبة الأدمن (Role Guard):**
> حقل `roles` يعود كنص يحتوي على الأدوار مفصولة بفواصل (مثل `"Admin"` أو `"Customer,Admin"`).
> يجب على الـ Admin Frontend التأكد فوراً من أن المستخدم يمتلك صلاحية `Admin`:
> ```typescript
> const userRoles = response.data.roles.split(',').map(r => r.trim());
> if (!userRoles.includes('Admin')) {
>   throw new Error("عفواً، لا تملك الصلاحيات الكافية للوصول إلى لوحة التحكم.");
> }
> ```

---

### 2. نسيان كلمة المرور — Forget Password
- **Endpoint:** `POST /api/v1/authentication/forget-password`
- **الوصف:** إرسال رمز OTP مكون من 6 أرقام إلى بريد الأدمن المسجل لاستعادة الحساب.

#### Request Body:
```json
{
  "email": "admin@example.com"
}
```

#### Success Response (`200 OK`):
```json
{
  "success": true,
  "isSuccess": true,
  "message": "Verification code sent successfully.",
  "statusCode": 200,
  "data": true,
  "errors": {}
}
```

#### Possible Errors:
- `400 Bad Request`: البريد غير صحيح أو فارغ (`EmailRequired`, `InvalidEmail`).
- `404 Not Found`: البريد غير مسجل بالنظام (`NotFound`).

---

### 3. التحقق من رمز الـ OTP — Verify OTP
- **Endpoint:** `POST /api/v1/authentication/verify-otp`
- **الوصف:** فحص صلاحية كود الـ OTP المكون من 6 أرقام. في حال صحته، يعود السيرفر بـ **Password Reset Token** لاستخدامه في الخطوة التالية.

#### Request Body:
```json
{
  "email": "admin@example.com",
  "otpCode": "849201"
}
```

#### Success Response (`200 OK`):
```json
{
  "success": true,
  "isSuccess": true,
  "message": "Verification code verified successfully.",
  "statusCode": 200,
  "data": "CfDJ8N3v1r6...[Long Reset Token String]...",
  "errors": {}
}
```

> [!NOTE]
> قم بحفظ قيمة الـ `data` (Reset Token) في State شاشة إعادة التعيين لتمريرها مع الخطوة التالية.

#### Possible Errors:
- `400 Bad Request`: الرمز غير صالح أو انتهت صلاحيته (`InvalidVerificationCode`, `VerificationCodeExpired`).

---

### 4. تعيين كلمة المرور الجديدة — Reset Password
- **Endpoint:** `POST /api/v1/authentication/reset-password`
- **الوصف:** تغيير كلمة المرور للمستخدم وإلغاء جميع الجلسات القديمة وتجديد إصدار التوكن (`TokenVersion`).

#### Request Body:
```json
{
  "email": "admin@example.com",
  "otpCode": "CfDJ8N3v1r6...[Reset Token or 6-digit OTP]...",
  "newPassword": "NewAdminPassword@2026",
  "confirmPassword": "NewAdminPassword@2026"
}
```

> [!TIP]
> حقل `otpCode` يدعم استقبال إما الـ `Reset Token` المسترجع من شاشة `verify-otp` (مفضل وموصى به)، أو كود الـ OTP المكون من 6 أرقام مباشرة إذا كانت صلاحيته لا تزال سارية.

#### Success Response (`200 OK`):
```json
{
  "success": true,
  "isSuccess": true,
  "message": "Password has been reset successfully.",
  "statusCode": 200,
  "data": true,
  "errors": {}
}
```

#### Possible Errors:
- `400 Bad Request`:
  - `NewPasswordCannotBeOldPassword`: لا يمكن استخدام كلمة المرور السابقة.
  - `PasswordsDoNotMatch`: كلمتا المرور غير متطابقتين.
  - `ResetTokenExpired`: انتهت صلاحية رمز إعادة التعيين.

---

### 5. تجديد التوكن — Refresh Token
- **Endpoint:** `POST /api/v1/authentication/refresh-token`
- **الوصف:** تجديد صلاحية الدخول عند انتهاء الـ Access Token باستخدام تقنية **Token Rotation** (يتم إبطال التوكن القديم وإصدار زوج جديد).

#### Request Body:
```json
{
  "refreshToken": "4a7b9c1d-8f2e-4b6a-9c3d-1e2f3a4b5c6d"
}
```

#### Success Response (`200 OK`):
```json
{
  "success": true,
  "isSuccess": true,
  "message": "Tokens refreshed successfully.",
  "statusCode": 200,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "9d8e7f6a-5b4c-3d2e-1f0a-9b8c7d6e5f4a"
  },
  "errors": {}
}
```

> [!WARNING]
> **مبدأ الـ Token Rotation:**
> عند نجاح هذا الطلب، يصبح الـ `refreshToken` القديم ملغياً فوراً (`Revoked`).
> يجب على الواجهة الأمامية تحديث الـ Storage بالـ `refreshToken` الجديد مباشرة.

#### Possible Errors:
- `401 Unauthorized` / `400 Bad Request`: التوكن منتهي الصلاحية أو مستخدم مسبقاً (`RefreshTokenExpired`, `RefreshTokenRevoked`). في هذه الحالة يجب توجيه المستخدم فوراً لتسجيل الدخول مجدداً.

---

### 6. تسجيل الخروج — Logout
- **Endpoint:** `POST /api/v1/authentication/logout`
- **الوصف:** إبطال الجلسة الحالية وحذف الـ Refresh Token من قاعدة البيانات لمنع استخدامه مستقبلاً.
- **التوثيق:** يتطلب `Authorization: Bearer <AccessToken>` (محمي بـ `[RoleAuthorize]`).

#### Request Body:
```json
{
  "refreshToken": "4a7b9c1d-8f2e-4b6a-9c3d-1e2f3a4b5c6d"
}
```
*(ملاحظة: يمكن إرسال `refreshToken: null` لإلغاء جميع الأجهزة النشطة للمستخدم الحساب).*

#### Success Response (`200 OK`):
```json
{
  "success": true,
  "isSuccess": true,
  "message": "Logout successful.",
  "statusCode": 200,
  "data": true,
  "errors": {}
}
```

---

## 4. كود تنفيذي متكامل للـ Frontend (Axios + Interceptors + Auto-Refresh)

فيما يلي بنية معيارية ومجربة لإدارة المصادقة والتعامل التلقائي مع انتهاء صلاحية الـ Tokens والأخطاء:

### تعريف الأنواع (TypeScript Interfaces)

```typescript
// types/auth.ts
export interface ApiResponse<T = any> {
  success: boolean;
  isSuccess: boolean;
  message: string | null;
  statusCode: number;
  data: T;
  errors: Record<string, string[]>;
}

export interface AuthResponseDto {
  accessToken: string;
  refreshToken: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  roles: string;
  id: string;
  profilePictureUrl?: string;
}

export interface RefreshTokenResponseDto {
  accessToken: string;
  refreshToken: string;
}
```

---

### إعداد Axios مع Interceptor للتجديد التلقائي للتوكن (Token Rotation)

```typescript
// api/apiClient.ts
import axios, { AxiosError, InternalAxiosRequestConfig } from "axios";
import { ApiResponse, RefreshTokenResponseDto } from "../types/auth";

const BASE_URL = process.env.REACT_APP_API_URL || "https://api.yourdomain.com";

export const apiClient = axios.create({
  baseURL: `${BASE_URL}/api/v1`,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

// إدارة صف الانتظار لتفادي استدعاء refresh-token عدة مرات عند تزامن أكثر من طلب
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (token: string) => void;
  reject: (error: any) => void;
}> = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach((promise) => {
    if (token) {
      promise.resolve(token);
    } else {
      promise.reject(error);
    }
  });
  failedQueue = [];
};

// 1. Request Interceptor: إضافة لغة الواجهة والتوكن
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // تحديد لغة الرسائل القادمة من الـ Global Exception Handler
    const currentLang = localStorage.getItem("app_lang") || "ar";
    config.headers["Accept-Language"] = currentLang;

    const token = localStorage.getItem("admin_access_token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// 2. Response Interceptor: التعامل مع Global Exceptions و 401 Unauthorized
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError<ApiResponse>) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

    // إذا كان الخطأ 401 والطلب لم تتم إعادة محاولته مسبقاً وليس طلب login أو refresh
    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      !originalRequest.url?.includes("/authentication/login") &&
      !originalRequest.url?.includes("/authentication/refresh-token")
    ) {
      if (isRefreshing) {
        // إذا كان هناك تجديد قائم بالفعل، ضع الطلب في قائمة الانتظار
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            return apiClient(originalRequest);
          })
          .catch((err) => Promise.reject(err));
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const currentRefreshToken = localStorage.getItem("admin_refresh_token");

      if (!currentRefreshToken) {
        handleForceLogout();
        return Promise.reject(error);
      }

      try {
        // استدعاء تجديد التوكن
        const { data: res } = await axios.post<ApiResponse<RefreshTokenResponseDto>>(
          `${BASE_URL}/api/v1/authentication/refresh-token`,
          { refreshToken: currentRefreshToken },
          {
            headers: {
              "Content-Type": "application/json",
              "Accept-Language": localStorage.getItem("app_lang") || "ar",
            },
          }
        );

        if (res.success && res.data) {
          const { accessToken, refreshToken } = res.data;

          // تحديث التخزين بالتوكنات الجديدة (Token Rotation)
          localStorage.setItem("admin_access_token", accessToken);
          localStorage.setItem("admin_refresh_token", refreshToken);

          apiClient.defaults.headers.common["Authorization"] = `Bearer ${accessToken}`;
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;

          processQueue(null, accessToken);
          return apiClient(originalRequest);
        } else {
          throw new Error("Failed to refresh token");
        }
      } catch (refreshError) {
        processQueue(refreshError, null);
        handleForceLogout();
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

function handleForceLogout() {
  localStorage.removeItem("admin_access_token");
  localStorage.removeItem("admin_refresh_token");
  localStorage.removeItem("admin_user");
  window.location.href = "/login?session_expired=true";
}
```

---

### ملف خدمات المصادقة (Auth Service)

```typescript
// services/authService.ts
import { apiClient } from "../api/apiClient";
import { ApiResponse, AuthResponseDto } from "../types/auth";

export const authService = {
  // 1. تسجيل الدخول
  async login(credentials: { email: string; password: string }): Promise<AuthResponseDto> {
    const response = await apiClient.post<ApiResponse<AuthResponseDto>>(
      "/authentication/login",
      credentials
    );

    const result = response.data;
    if (!result.success || !result.data) {
      throw result;
    }

    // التحقق الصارم من رتبة الأدمن
    const roles = result.data.roles ? result.data.roles.split(",").map((r) => r.trim()) : [];
    if (!roles.includes("Admin")) {
      throw {
        message: "عفواً، لا تمتلك صلاحيات الأدمن للوصول إلى هذه اللوحة.",
        statusCode: 403,
      };
    }

    // حفظ التوكنات وبيانات المستخدم
    localStorage.setItem("admin_access_token", result.data.accessToken);
    localStorage.setItem("admin_refresh_token", result.data.refreshToken);
    localStorage.setItem("admin_user", JSON.stringify(result.data));

    return result.data;
  },

  // 2. نسيان كلمة المرور
  async forgetPassword(email: string): Promise<boolean> {
    const response = await apiClient.post<ApiResponse<boolean>>(
      "/authentication/forget-password",
      { email }
    );
    return response.data.data;
  },

  // 3. التحقق من كود الـ OTP والحصول على Reset Token
  async verifyOtp(email: string, otpCode: string): Promise<string> {
    const response = await apiClient.post<ApiResponse<string>>(
      "/authentication/verify-otp",
      { email, otpCode }
    );
    return response.data.data;
  },

  // 4. تعيين كلمة المرور الجديدة
  async resetPassword(payload: {
    email: string;
    otpCode: string; // Token المستلم من verifyOtp
    newPassword: string;
    confirmPassword: string;
  }): Promise<boolean> {
    const response = await apiClient.post<ApiResponse<bool>>(
      "/authentication/reset-password",
      payload
    );
    return response.data.data;
  },

  // 5. تسجيل الخروج
  async logout(): Promise<void> {
    const refreshToken = localStorage.getItem("admin_refresh_token");
    try {
      await apiClient.post<ApiResponse<boolean>>("/authentication/logout", {
        refreshToken,
      });
    } finally {
      localStorage.removeItem("admin_access_token");
      localStorage.removeItem("admin_refresh_token");
      localStorage.removeItem("admin_user");
      window.location.href = "/login";
    }
  },
};
```

---

## 5. إرشادات أمنية هامة للـ Admin Frontend

1. **التحقق من الصلاحيات في جهة العميل (Route Guarding):**
   - لا تكتفِ فقط بفحص وجود الـ Token في `localStorage`، بل تأكد دائماً من أن الـ User يحتوي على دور `Admin`.
2. **التعامل مع إبطال الجلسة (`SessionRevoked`):**
   - عندما يقوم أي مستخدم بتغيير كلمة مروره أو يقوم المشرف بتعديل صلاحياته، يقوم السيرفر بزيادة `TokenVersion` وإبطال كل الجلسات. سيعيد الخادم `401 Unauthorized` فوراً للطلبات اللاحقة؛ لذا تم إعداد الـ Interceptor لإنهاء الجلسة بسلاسة وتوجيه المستخدم لشاشة تسجيل الدخول.
3. **تزامن التوكن وتجنب التعارض (Concurrency & Race Conditions):**
   - بفضل استخدام صف الانتظار `failedQueue` في كود الـ Axios Interceptor، إذا أطلقت الصفحة 5 طلبات متزامنة في لحظة انتهاء الـ Access Token، فسيتم إرسال طلب واحد فقط لـ `/refresh-token`، وستنتظر باقي الطلبات التوكن الجديد ثم تنطلق بنجاح دون أي خطأ 401 عشوائي.
4. **لغات النظام:**
   - احرص على إرسال الـ `Accept-Language` مع كل طلب حتى تظهر رسائل الـ Validation للمديرين بلغتهم المفضلة (العربية أو الإنجليزية).
