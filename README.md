# Oqtane ContactMe Module

A flexible, secure, and feature-rich contact form module for the Oqtane framework. Effortlessly connect with your audience, manage submissions, and protect your site from spam.

---

## Features

### 📝 Fully Customizable Forms
- **Custom Instructions:** Display a unique message or instructions at the top of your form to guide your users.
- **Personalized Success Messages:** Configure a custom "Thank You" message that appears after a successful submission, supporting line breaks and custom formatting.
- **Dynamic Interest Checklists:**  
  - Easily create a checklist of interests, topics, or services from the module settings.
  - Enter a list of items into the "Interest List" text area, one per line, and the module automatically transforms this into a user-friendly set of checkboxes.
  - Capture structured feedback to better understand your users' needs.

### 🗂️ Powerful Submission Management
- **Centralized Viewing:** All submissions are collected in a clean, paginated grid, sorted with the most recent entries first.
- **Detailed Record View:** Click "View" to see the complete details of any submission, including all form fields and metadata.
- **Easy Deletion:** Securely remove old or irrelevant submissions with a simple, one-click action.

### 📧 Automated Email Notifications
- **Instant Alerts:** Automatically send a detailed email notification to a designated user as soon as a form is submitted.
- **Configurable Recipient:** Select any registered user on your site to receive notifications from the module settings.
- **Complete Submission Details:** Notification emails contain all submitted information, including the user's IP address.

### 🛡️ Robust Security Built-In
- **Anti-Bot Honeypot:** An invisible field traps and discards submissions from automated bots.
- **Cross-Site Scripting (XSS) Protection:** All user input is sanitized on the server to prevent malicious scripts.
- **SQL Injection Protection:** Built on Entity Framework Core for inherent protection.
- **Smart URL Formatting:** Automatically corrects user-entered website links to ensure they are valid and clickable.

### ⚙️ Seamless Oqtane Integration
- **Searchable Content:** Submissions are indexed by Oqtane's search engine.
- **Portable:** Supports Oqtane's import/export functionality for easy migration.
- **Standard & Reliable:** Follows Oqtane best practices for installation, migrations, and module management.

---

## Installation

1. Log in to your Oqtane instance as a **Host** user.
2. Go to **Admin Dashboard** > **Module Management**.
3. Click **Install Module** and upload the module's NuGet package.
4. Add the module to any page on your site.

## Configuration

After adding the module, access settings to configure its behavior:
- **Instructions:** Enter any text or HTML to display above the form.
- **Success Feedback:** Enter the message to display after a successful submission.
- **Send To User:** Select a registered user to receive email notifications.
- **Interest List:** Enter a list of items, one per line, to be rendered as checkboxes on the form.

---

## License

This module is licensed under the [MIT License](LICENSE.md).
