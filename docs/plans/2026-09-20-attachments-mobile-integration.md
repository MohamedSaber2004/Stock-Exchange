# Attachments API Mobile Integration Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use subagent-driven-development (recommended) or executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Document how mobile developers integrate with the Attachments API endpoints on the test server, including routes, request formats, response formats, and file fetching/download flows.

**Architecture:** Mobile apps call REST endpoints on `https://stock-exchange.runasp.net` using Bearer token authentication. Uploads use `multipart/form-data`, downloads use a query-string GET endpoint that returns file bytes, and already-uploaded files can also be loaded directly from the public static file URL under `/files/General/<filename>`.

**Tech Stack:** ASP.NET Core 8, REST/HTTP, multipart/form-data, Bearer token auth, Swagger UI, static file serving

---

## File Structure Map

| File | Responsibility |
|------|---------------|
| `docs/plans/2026-09-20-attachments-mobile-integration.md` | This plan document |
| `docs/attachments-mobile-integration.md` | Mobile developer integration guide (created during implementation) |
| `Stock Exchange/Controllers/V1/AttachmentsController.cs` | API endpoint definitions |
| `Stock Exchange/Routes/V1/ApiRoutes.cs` | Route constants |
| `Stock Exchange/Routes/BaseRoutes.cs` | Base route pattern |

---

## Endpoint Reference

**Base URL:** `https://stock-exchange.runasp.net`

### 1. Upload Single File

**Route:** `POST /api/v1/attachments/upload`

**Content-Type:** `multipart/form-data`

**Request Body (form-data):**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `File` | file | Yes | Binary file to upload |
| `MediaType` | int | Yes | `0` = Image, `1` = Video, `2` = Audio, `3` = File/Document |
| `Place` | int | Yes | `0` = General storage folder |

**Response Format (201 Created):**

```json
{
  "isSuccess": true,
  "statusCode": 201,
  "message": "Created successfully",
  "data": "0_<guid>.jpg"
}
```

**Error Response (400 Bad Request):**

```json
{
  "isSuccess": false,
  "statusCode": 400,
  "message": "Invalid file format",
  "errors": ["Invalid file format"]
}
```

### 2. Upload Multiple Files

**Route:** `POST /api/v1/attachments/upload-multiple`

**Content-Type:** `multipart/form-data`

**Request Body (form-data):**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `Files` | array[files] | Yes | Multiple files (use the same field name `Files` for each file) |
| `MediaType` | int | Yes | `0` = Image, `1` = Video, `2` = Audio, `3` = File/Document |
| `Place` | int | Yes | `0` = General storage folder |

**Response Format (201 Created):**

```json
{
  "isSuccess": true,
  "statusCode": 201,
  "message": "Created successfully",
  "data": "0_<guid1>.jpg,0_<guid2>.jpg"
}
```

### 3. Download File

**Route:** `GET /api/v1/attachments/download`

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `FileName` | string | Yes | Stored file name returned from upload, e.g. `0_<guid>.jpg` |
| `FilePlace` | int | Yes | `0` = General storage folder |
| `MediaType` | int | Yes | `0` = Image, `1` = Video, `2` = Audio, `3` = File/Document |

**Response Format (200 OK):**

- Content-Type: `image/jpeg`, `application/pdf`, etc. based on file extension
- Body: raw file bytes (binary)
- Content-Disposition: attachment with the original file name

**Error Response (404 Not Found):**

```json
{
  "isSuccess": false,
  "statusCode": 404,
  "message": "File not found",
  "errors": ["File not found"]
}
```

### 4. Update / Replace File

**Route:** `PUT /api/v1/attachments/update`

**Content-Type:** `multipart/form-data`

**Request Body (form-data):**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `OldFileName` | string | Yes | Existing stored file name to replace, e.g. `0_<guid>.jpg` |
| `File` | file | Yes | New binary file to upload |
| `MediaType` | int | Yes | `0` = Image, `1` = Video, `2` = Audio, `3` = File/Document |
| `Place` | int | Yes | `0` = General storage folder |

**Response Format (200 OK):**

```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Updated successfully",
  "data": "0_<new-guid>.jpg"
}
```

---

## Mobile Integration Flow

### Upload → Display → Download

**Step 1: Upload a file**

Send `POST /api/v1/attachments/upload` with multipart/form-data. Save the `data` value from the response. This is the stored file name.

**Step 2: Display the file in the app**

After upload, construct the static file URL by stripping the place prefix from the stored name:

- Stored name: `0_<guid>.jpg`
- Static URL: `https://stock-exchange.runasp.net/files/General/<guid>.jpg`

The static file URL is public and can be used directly in image views, video players, or audio players.

**Step 3: Download the file**

Call `GET /api/v1/attachments/download?FileName=0_<guid>.jpg&FilePlace=0&MediaType=0` with the Bearer token. The response body is the raw file bytes.

---

## Mobile Code Examples

### cURL

```bash
curl -X POST "https://stock-exchange.runasp.net/api/v1/attachments/upload" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "File=@/path/to/image.jpg" \
  -F "MediaType=0" \
  -F "Place=0"
```

```bash
curl -L "https://stock-exchange.runasp.net/api/v1/attachments/download?FileName=0_<guid>.jpg&FilePlace=0&MediaType=0" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -o downloaded.jpg
```

### Swift (iOS)

```swift
let url = URL(string: "https://stock-exchange.runasp.net/api/v1/attachments/upload")!
var request = URLRequest(url: url)
request.httpMethod = "POST"
request.setValue("Bearer \(token)", forHTTPHeaderField: "Authorization")

let boundary = UUID().uuidString
request.setValue("multipart/form-data; boundary=\(boundary)", forHTTPHeaderField: "Content-Type")

var body = Data()
body.append("--\(boundary)\r\n".data(using: .utf8)!)
body.append("Content-Disposition: form-data; name=\"File\"; filename=\"image.jpg\"\r\n".data(using: .utf8)!)
body.append("Content-Type: image/jpeg\r\n\r\n".data(using: .utf8)!)
body.append(imageData)
body.append("\r\n".data(using: .utf8)!)

body.append("--\(boundary)\r\n".data(using: .utf8)!)
body.append("Content-Disposition: form-data; name=\"MediaType\"\r\n\r\n".data(using: .utf8)!)
body.append("0\r\n".data(using: .utf8)!)

body.append("--\(boundary)\r\n".data(using: .utf8)!)
body.append("Content-Disposition: form-data; name=\"Place\"\r\n\r\n".data(using: .utf8)!)
body.append("0\r\n".data(using: .utf8)!)

body.append("--\(boundary)--\r\n".data(using: .utf8)!)
request.httpBody = body
```

### Kotlin (Android)

```kotlin
val url = "https://stock-exchange.runasp.net/api/v1/attachments/upload"
val requestBody = MultipartBody.Builder()
    .setType(MultipartBody.FORM)
    .addFormDataPart("File", "image.jpg", file.asRequestBody("image/jpeg".toMediaTypeOrNull()))
    .addFormDataPart("MediaType", "0")
    .addFormDataPart("Place", "0")
    .build()

val request = Request.Builder()
    .url(url)
    .post(requestBody)
    .addHeader("Authorization", "Bearer $token")
    .build()
```

---

## Important Notes for Mobile Developers

- **File naming convention:** Uploaded files are stored with a GUID name (e.g., `<guid>.jpg`). The API prefixes the stored name with the place ID (e.g., `0_<guid>.jpg`). When building static URLs, strip the `{Place}_` prefix before the first underscore.
- **Allowed file types:** Images (`.jpg`, `.jpeg`, `.png`, `.gif`, `.bmp`, `.webp`), Videos (`.mp4`, `.avi`, `.mkv`, `.mov`, `.wmv`), Audio (`.mp3`, `.wav`, `.ogg`, `.m4a`, `.aac`), Documents (`.pdf`, `.doc`, `.docx`, `.xls`, `.xlsx`, `.txt`, `.zip`, `.rar`)
- **Storage location:** All files for `Place=0` are stored in `wwwroot/General/` and served publicly at `/files/General/<filename>`.
- **Authentication:** Upload and download endpoints require the `Authorization: Bearer <token>` header. Static file URLs are public.
- **Rate limiting:** 60 requests/minute for API routes.
- **Error handling:** Check the `isSuccess` field in JSON responses. If `false`, read the `message` and `errors` fields.

---

## Verification Steps

- [ ] Open `https://stock-exchange.runasp.net/swagger/` and confirm Swagger UI loads.
- [ ] Confirm the root URL `https://stock-exchange.runasp.net/` redirects to `/swagger/`.
- [ ] Upload a test image via `POST /api/v1/attachments/upload` and verify a `201` response with a stored file name.
- [ ] Load the uploaded file in a browser via `https://stock-exchange.runasp.net/files/General/<guid>.jpg`.
- [ ] Download the file via `GET /api/v1/attachments/download?FileName=0_<guid>.jpg&FilePlace=0&MediaType=0` and verify the response is binary content.
- [ ] Replace the file via `PUT /api/v1/attachments/update` and verify a `200` response with the new stored file name.
