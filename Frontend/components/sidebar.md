# Sidebar Component

The `Sidebar` is a reusable navigation panel for TalentFlow. It renders the app logo, grouped nav tabs with optional notification badges, and a user profile section at the bottom.

**Location:** `app/components/Sidebar.tsx`

---

## Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `badges` | `Record<string, number>` | `{}` | Badge counts keyed by tab label. Only tabs with a count > 0 show a badge. |
| `activeTab` | `string` | `"Dashboard"` | The currently active tab label. Controls which tab is highlighted. |

Both props are optional — the component works out of the box with no props.

---

## Basic Usage

Import and render the `Sidebar` inside a flex layout. It takes up `25.26vw` width (min `250px`) and fills the full viewport height.

```tsx
import Sidebar from "@/app/components/Sidebar";

export default function Page() {
  return (
    <div className="flex">
      <Sidebar />
      <main className="flex-1">
        {/* Page content */}
      </main>
    </div>
  );
}
```

---

## Setting the Active Tab

Pass the `activeTab` prop with the **exact** tab label to highlight it. The active tab gets a solid `#3E5C8E` background.

```tsx
<Sidebar activeTab="Assignments" />
```

### Available tab labels

These are the only valid values for `activeTab`:

| Section | Tab Labels |
|---------|-----------|
| **MAIN** | `"Dashboard"`, `"Course Catalog"`, `"Assignments"`, `"My Progress"` |
| **COMMUNITY** | `"Discussions"`, `"My Team"`, `"Notifications"` |
| **ACCOUNT** | `"Profile"`, `"Certificates"` |

> ⚠️ Labels are **case-sensitive** and must match exactly (e.g. `"My Progress"`, not `"my progress"`).

---

## Adding Notification Badges

Pass a `badges` object where each key is a tab label and the value is the notification count. Only entries with a count **greater than 0** are displayed.

```tsx
<Sidebar
  activeTab="Dashboard"
  badges={{
    Assignments: 4,
    Discussions: 6,
    Notifications: 9,
  }}
/>
```

To show badges on **every** tab:

```tsx
<Sidebar
  activeTab="Dashboard"
  badges={{
    Dashboard: 3,
    "Course Catalog": 12,
    Assignments: 4,
    "My Progress": 7,
    Discussions: 6,
    "My Team": 2,
    Notifications: 9,
    Profile: 1,
    Certificates: 5,
  }}
/>
```

To show **no badges**, simply omit the prop or pass an empty object:

```tsx
<Sidebar activeTab="Dashboard" badges={{}} />
```

---

## Full Example

A typical page layout combining both props:

```tsx
import Sidebar from "@/app/components/Sidebar";

export default function AssignmentsPage() {
  return (
    <div className="flex min-h-screen">
      <Sidebar
        activeTab="Assignments"
        badges={{
          Assignments: 4,
          Notifications: 9,
          Discussions: 6,
        }}
      />
      <main className="flex-1 bg-[#0D1117] p-8">
        <h1 className="text-white text-2xl">Assignments</h1>
        {/* Page content here */}
      </main>
    </div>
  );
}
```

---

## Structure Overview

The sidebar is split into three visual sections:

```
┌──────────────────────┐
│  TalentFlow (logo)   │
├──────────────────────┤
│  MAIN                │
│    Dashboard         │
│    Course Catalog    │
│    Assignments   [4] │
│    My Progress       │
│                      │
│  COMMUNITY           │
│    Discussions   [6] │
│    My Team           │
│    Notifications [9] │
│                      │
│  ACCOUNT             │
│    Profile           │
│    Certificates      │
│                      │
│  ···  (spacer)  ···  │
├──────────────────────┤
│  [AO] Adeeze okoro ⚙│
│       UI/UX Intern   │
└──────────────────────┘
```

---

## Icon Assets

All icons live in `public/assets/` and follow the `*_icon.svg` naming convention:

| Tab | Icon file |
|-----|-----------|
| Dashboard | `dashboard_icon.svg` |
| Course Catalog | `course_catalog_icon.svg` |
| Assignments | `assignment_icon.svg` |
| My Progress | `my_progress_icon.svg` |
| Discussions | `discussions_icon.svg` |
| My Team | `my_team_icon.svg` |
| Notifications | `notification_icon.svg` |
| Profile | `profile_icon.svg` |
| Certificates | `certificates_icon.svg` |
| Settings (bottom) | `setting_wheel_icon.svg` |

---

## Element IDs (for testing)

Every interactive element has a unique `id` for browser/e2e testing:

| Element | ID pattern | Example |
|---------|-----------|---------|
| Sidebar container | `sidebar-nav` | — |
| Nav tab button | `nav-{label-kebab}` | `nav-course-catalog` |
| Settings button | `settings-button` | — |
