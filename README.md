# SmartClinic Mobile

SmartClinic Mobile is the Android companion application developed for the **Smart Clinic Management System** as part of the **INSY7315 POE Part 2** project.

The application was developed using **.NET MAUI and C#** and provides patients with a mobile interface for managing appointments, viewing clinical information, receiving notifications, updating their profile, and completing clinic-related tasks.

The mobile application complements the group's main SmartClinic website while operating as a standalone patient-facing application with its own local SQLite database.

---

## Project Information

**Module:** INSY7315  
**Assessment:** POE Part 2  
**Project:** Smart Clinic Management System  
**Mobile Application:** SmartClinic Mobile  
**Student:** Raghav Mahraj  
**Student Number:** ST10458336  

---

## Project Demonstration Videos

The following demonstration and presentation videos form part of the final project submission.

**Main Website YouTube Video:**  
_To be added_

**Mobile App YouTube Video:**  
_To be added_

**Group Presentation YouTube Video:**  
_To be added_

---

## Android APK Download

A packaged Android version of SmartClinic Mobile is available through the repository's **GitHub Releases** section.

### Installation

1. Open the **Releases** section of this GitHub repository.
2. Open the latest SmartClinic Mobile release.
3. Download the signed Android APK.
4. Transfer/open the APK on an Android device if necessary.
5. Android may request permission to install applications from the selected source.
6. Install the APK.
7. Open SmartClinic Mobile and use the demo account below.

> The APK is distributed directly through GitHub for project demonstration and assessment purposes and is not distributed through the Google Play Store.

### Demo Account

**Email:** `demo.patient@smartclinic.local`  
**Password:** `Demo@12345`

The demo account contains sample clinic information that can be used to demonstrate the application's patient workflows.

---

## Mobile Application Features

SmartClinic Mobile provides the patient-facing functionality of the Smart Clinic Management System.

### Authentication and Account Management

Patients can:

- Register a new account.
- Sign in using their email address and password.
- Access functionality associated with the authenticated patient.
- Update their personal profile information.
- Sign out securely.
- Return to their saved information after reopening the application.

Passwords are not stored as plain text in the application database. Password hashing and verification are handled through the application's authentication service.

### Patient Dashboard

The dashboard provides a central starting point for the patient and displays relevant information and shortcuts to the major areas of the application.

Patients can access:

- Upcoming appointments.
- Appointment booking.
- Check-in and queue information.
- Consultation history.
- Prescriptions.
- Notifications.
- Patient profile information.

### Doctor Availability

Patients can view available doctors and their appointment availability.

The application includes doctor information such as:

- Doctor name.
- Specialisation.
- Qualification.
- Available appointment dates and times.

Appointment availability is stored in the local database and updated when appointment slots are booked, cancelled or rescheduled.

### Appointment Management

Patients can manage the appointment lifecycle from within the application.

Supported functionality includes:

- Viewing upcoming appointments.
- Booking an available appointment.
- Selecting a doctor and available time slot.
- Providing a reason for the visit.
- Rescheduling an existing appointment.
- Cancelling an appointment.
- Viewing appointment status.
- Receiving appointment confirmation information.

Business rules prevent invalid appointment operations such as booking unavailable slots or managing appointments that are no longer eligible for modification.

### Digital Check-In and Queue

SmartClinic Mobile includes a patient check-in and queue workflow.

The application can:

- Determine whether an appointment is eligible for check-in.
- Prevent check-in outside the permitted time window.
- Update appointment status after check-in.
- Add the patient to the local clinic queue.
- Display the patient's queue status.
- Maintain queue information between application sessions.

### Consultation History

Patients can access their previous consultation records through the application.

Consultation information includes relevant clinical details associated with the authenticated patient and can be opened from the consultation history screen for further information.

### Digital Prescriptions

Patients can view prescriptions associated with their consultation records.

Prescription information is retrieved from the local SmartClinic database and restricted to the currently authenticated patient's records.

### Notifications and Reminders

The application includes a patient notification centre for clinic-related information.

Patients can:

- View notifications.
- Mark individual notifications as read.
- Mark all notifications as read.
- Retain notification status between application sessions.
- Receive appointment and follow-up information represented within the SmartClinic workflow.

---

## Technology Stack

SmartClinic Mobile was developed using:

| Technology | Purpose |
| --- | --- |
| .NET MAUI | Cross-platform mobile application framework |
| C# | Application programming language |
| XAML | Mobile user interface design |
| .NET 10 | Application framework/runtime |
| SQLite | Local persistent relational data storage |
| sqlite-net-pcl | SQLite data access |
| Microsoft PasswordHasher | Password hashing and verification |
| .NET MAUI SecureStorage | Authenticated session storage |
| xUnit | Automated testing |
| Git | Source control |
| GitHub | Repository and project version control |
| GitHub Actions | Continuous Integration and build automation |

The submitted mobile implementation targets **Android**.

---

## Application Architecture

The mobile application follows a structured separation between the user interface, application logic, services and persistence layer.

A simplified application flow is:

```text
Views (XAML)
      |
      v
ViewModels
      |
      v
Application Services / Business Rules
      |
      v
SmartClinicDatabase
      |
      v
SQLite Database
```

This structure separates presentation concerns from business logic and database operations, making the application easier to maintain and test.

### Views

The Views contain the XAML user interfaces presented to the patient.

Examples include:

- Login
- Registration
- Dashboard
- Appointments
- Appointment Booking
- Appointment Rescheduling
- Check-In and Queue
- Consultation History
- Consultation Details
- Prescriptions
- Notifications
- Patient Profile

### ViewModels

ViewModels manage page state, user actions and communication between the user interface and application services.

### Services

The service layer contains the application's operational and business logic, including:

- Authentication.
- Patient management.
- Doctor availability.
- Appointment management.
- Queue management.
- Clinical information.
- Notifications.

### Models

Models represent the data used throughout the application, including users, patients, doctors, appointments, consultations, prescriptions, notifications and queue information.

---

## Local SQLite Database

SmartClinic Mobile uses a local SQLite database named:

```text
smartclinic.db3
```

The database is created inside the application's local data directory when required.

The database contains tables for:

- User
- Patient
- Doctor
- DoctorAvailability
- Appointment
- Consultation
- Prescription
- Notification
- QueueStatus

SQLite allows the mobile application to maintain persistent patient data without requiring an active connection to the main SmartClinic website.

When the packaged APK is installed on another Android device, that installation creates and manages its own local SmartClinic database.

---

## Demo Data

The application creates demonstration data to allow the major patient workflows to be evaluated without requiring manual data entry before the demonstration.

The seeded environment includes:

- Demo patient account.
- Doctors.
- Doctor availability.
- Historical appointments.
- Consultation records.
- Prescription information.
- Patient notifications.
- Follow-up information.
- A future appointment.

Newly registered patient accounts are kept separate from the demo patient's information.

---

## Security

Security was considered throughout the mobile implementation.

### Password Security

User passwords are hashed before being stored in SQLite.

Authentication verifies the submitted password against the stored password hash rather than comparing or storing plain-text passwords.

### Secure Session Storage

Authenticated session information is stored using **.NET MAUI SecureStorage** rather than being stored as plain application data.

### Patient Data Isolation

Patient-facing services use the authenticated patient identity when retrieving records.

This prevents one registered patient from simply receiving another patient's appointments, consultations, prescriptions or other patient-specific information through the normal application workflows.

### Input Validation

Validation is applied to important operations including:

- Registration.
- Login.
- Appointment creation.
- Appointment rescheduling.
- Appointment cancellation.
- Profile updates.
- Appointment check-in.

The application provides feedback when an operation cannot be completed.

---

## Testing

SmartClinic Mobile was tested using both automated and manual testing.

### Automated Testing

The project includes an **xUnit automated test project**:

```text
SmartClinic.Mobile.Tests
```

The automated suite contains **61 tests** covering important application and business rules.

Current result:

```text
Total tests: 61
Passed:      61
Failed:      0
Skipped:     0
```

The automated tests are also executed by the GitHub Actions CI/CD workflow.

### Functional Testing

The Android application was manually tested using an Android emulator.

Tested workflows include:

- Registration.
- Valid and invalid login.
- Duplicate account validation.
- Patient data isolation.
- Profile editing.
- Appointment booking.
- Appointment rescheduling.
- Appointment cancellation.
- Doctor availability.
- Check-in eligibility.
- Queue status.
- Consultation history.
- Consultation details.
- Prescriptions.
- Notifications.
- Notification read status.
- Logout.
- Persistent SQLite data.

### Packaged APK Testing

The Android APK generated by GitHub Actions was downloaded and installed independently from Visual Studio.

The packaged application was verified to:

- Install successfully.
- Launch successfully.
- Initialise its local SQLite data.
- Authenticate using the demo account.
- Load the patient dashboard.
- Access patient functionality.

This verifies that the distributable build operates independently from the Visual Studio development environment.

---

## CI/CD with GitHub Actions

SmartClinic Mobile uses **GitHub Actions** for automated Continuous Integration and build packaging.

The workflow is stored under:

```text
.github/workflows/ci.yml
```

The pipeline performs the following process:

```text
Source Code Push
      |
      v
Checkout Repository
      |
      v
Configure .NET 10
      |
      v
Install .NET MAUI Android Workload
      |
      v
Restore Dependencies
      |
      v
Build Android Application
      |
      v
Run Automated Tests
      |
      v
Publish Android APK
      |
      v
Upload APK Build Artifact
```

The workflow runs for relevant pushes to:

- `feature/**`
- `develop`
- `main`

It also validates pull requests targeting the main development branches.

A failed build or automated test causes the workflow to fail, providing an automated verification stage before final release.

---

## Git Branching Strategy

Development was completed using separate Git branches for major areas of functionality.

Examples include:

```text
main
develop

feature/authentication
feature/patient-dashboard
feature/check-in-queue
feature/clinical-records
feature/notifications
feature/patient-profile
feature/mobile-polish
feature/final-ui-branding
feature/sqlite-persistence
feature/automated-testing
feature/ci-cd
```

Feature work is integrated into `develop` before the completed submission version is merged into `main`.

This approach keeps individual areas of development separated while maintaining a stable final branch.

---

## User Interface and Branding

The mobile application uses a shared SmartClinic design system to maintain a consistent visual identity.

The interface includes:

- SmartClinic branding.
- Consistent blue and medical-red accent colours.
- Reusable typography styles.
- Consistent cards and form controls.
- Success and error feedback.
- Accessible touch targets.
- Semantic accessibility properties.
- Clear navigation.
- Loading and activity feedback.
- Consistent page layouts.

The interface was designed specifically for a mobile patient workflow rather than reproducing the desktop website interface.

---

## Relationship to the Main SmartClinic Website

The group implementation consists of both a main website and the SmartClinic mobile application.

The **website is the group's primary hosted implementation** and provides the broader Smart Clinic Management System.

SmartClinic Mobile is the **patient-facing Android companion application** developed using .NET MAUI.

For this implementation, the mobile application uses its own local SQLite persistence layer. It does **not claim live synchronisation with the website's hosted database**.

This separation allows the mobile implementation to demonstrate complete patient workflows, local persistence, authentication, security, business logic, automated testing and Android deployment while the group's main website provides the primary hosted system implementation.

---

## Running the Project from Source

### Requirements

To work with the source project, the development environment requires:

- Visual Studio with .NET MAUI support.
- .NET 10 SDK.
- Android SDK.
- Android emulator or compatible Android device.

### Running SmartClinic Mobile

1. Clone the repository.
2. Open the SmartClinic Mobile solution in Visual Studio.
3. Restore the required NuGet packages.
4. Select an Android emulator/device.
5. Build the solution.
6. Run `SmartClinic.Mobile`.

The local SQLite database will be initialised by the application when required.

---

## Running the Automated Tests

The automated tests can be executed through Visual Studio Test Explorer.

Alternatively, the test project can be executed as part of the repository's automated GitHub Actions workflow.

A successful CI run verifies both the Android build and automated test suite before the APK artifact is produced.

---

## Repository Structure

```text
INSY7315-POE-Part-2-SmartClinic-Mobile/
|
|-- .github/
|   `-- workflows/
|       `-- ci.yml
|
|-- SmartClinic.Mobile/
|   |-- Data/
|   |-- Models/
|   |-- Resources/
|   |-- Services/
|   |-- ViewModels/
|   |-- Views/
|   `-- SmartClinic.Mobile.csproj
|
|-- SmartClinic.Mobile.Tests/
|   `-- Automated test project
|
|-- README.md
`-- .gitignore
```

The exact contents of individual folders may expand as the project is maintained.

---

## Project Status

SmartClinic Mobile currently includes:

- Completed patient-facing Android interface.
- Local SQLite persistence.
- Authentication and patient account management.
- Appointment management.
- Doctor availability.
- Check-in and queue workflow.
- Consultation history.
- Digital prescriptions.
- Notifications and reminders.
- Patient profile management.
- Automated business-rule testing.
- GitHub Actions CI/CD.
- Automated Android Release build.
- Downloadable Android APK.
- Successful standalone APK installation testing.

---

## Submission Notes

SmartClinic Mobile forms the mobile component of the group's Smart Clinic Management System submission.

The repository demonstrates the mobile application's source code, version-control history, automated tests, CI/CD configuration and distributable Android build.

For the complete SmartClinic system demonstration, refer to the website, mobile application and group presentation videos listed at the beginning of this README.

---

## Author

**Raghav Mahraj**  
**Student Number:** ST10458336  
**Module:** INSY7315  
**Assessment:** POE Part 2  
**Project:** Smart Clinic Management System
