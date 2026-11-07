# Dalil Academy LMS API — Lab 08: JWT Authentication (Lock It Down)

ASP.NET Core 10 · EF Core 10 · PostgreSQL · Onion Architecture (Core / Infra / API) · FluentValidation · JWT · BCrypt

**Lecture 08: Authentication with JWT** · migrations `AddUsers` + `LinkStudentsToUsers` · tested on your PC: **33 / 33 checks passed**.

## Logins (seeded)
| Email | Password | Role |
|-------|----------|------|
| admin@dalil.jo | Admin@123 | Admin |
| instructor@dalil.jo | Instructor@123 | Instructor |
| *(register your own)* | — | Student |

Passwords are stored only as BCrypt hashes (`$2a$11$...`). **Jwt:Key** is in user-secrets on your PC (64 random chars), never in the project.
To see it: `dotnet user-secrets list --project DalilAcademy.API`

## Lab tasks
| # | Task | Result |
|---|------|--------|
| 01 | `POST /api/auth/register` → Student role, hashed password, returns a token | 201 · duplicate 409 · weak password 400 |
| 02 | `Student.UserId` (nullable, unique FK → Users, `ON DELETE SET NULL`) | migration `LinkStudentsToUsers` |
| 03 | `StudentsController` = `[Authorize(Roles = Roles.Admin)]` | admin 200/201/204 · student 403 · none 401 |
| 04 | `GET /api/enrollments/me` (id from the token, never the URL) | student 200 · admin 403 · none 401 |
| 05 | Token expiry: `ExpiresInHours = 0.02` (72 s) + 1 min ClockSkew | 200 right away → **401 after 3 minutes** (`The token expired at ...`) |

Task 05 in Visual Studio: change `"ExpiresInHours": 8` to `0.02` in appsettings.json, restart, log in, wait 3 minutes — then set it back to 8.
(The test script does the same with the environment variable `Jwt__ExpiresInHours=0.02`, without editing the file.)

## Who can do what
| Endpoint | None | Student | Instructor | Admin |
|----------|------|---------|------------|-------|
| GET /api/courses, /courses/{id}, /courses/count | ✅ | ✅ | ✅ | ✅ |
| POST / DELETE /api/courses | 401 | 403 | 403 | ✅ |
| PUT /api/courses/{id} | 401 | 403 | ✅ | ✅ |
| /api/students (all) | 401 | 403 | 403 | ✅ |
| POST /api/enrollments (any student) | 401 | 403 | 403 | ✅ |
| GET / POST /api/enrollments/me | 401 | ✅ | 403 | 403 |
| POST /api/auth/login, /register | ✅ | ✅ | ✅ | ✅ |

Also verified: a student who edits `"role":"Student"` → `"Admin"` in the token gets **401** (signature mismatch).

## Testing
- **Swagger:** F5 → https://localhost:7001 (or http://localhost:5147) → `POST /api/auth/login` → copy `token` → **Authorize** button → paste.
- **Postman:** import `Lab08_Lock_It_Down.postman_collection.json` → Run. Logins save `adminToken`, `instructorToken`, `studentToken` automatically.
- **Results:** `Lab08_Test_Results.xlsx` — Part 1 (16/16) and Part 2 video scenarios (10/10), all PASS.

## HTTPS
`UseHttpsRedirection()` + an `https` profile (https://localhost:7001). First time only, trust the dev certificate:
`dotnet dev-certs https --trust` → click **Yes** in the Windows dialog.

## Run it
1. PostgreSQL on `localhost:5432` — connection string in `DalilAcademy.API/appsettings.json` (local dev credentials).
2. Set the JWT signing key once (stored outside the repo, never committed):
   `dotnet user-secrets set "Jwt:Key" "<32+ random characters>" --project DalilAcademy.API`
3. Apply migrations: `dotnet ef database update --project DalilAcademy.Infra --startup-project DalilAcademy.API`
4. Open `DalilAcademy.slnx` → **http** profile → **F5** → Swagger at http://localhost:5147

`ExpiresInHours` is set to **0.02** (72 s) for the token-expiration requirement; set it to 8 for normal use.
