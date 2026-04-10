"use client";

import { useState, useEffect } from "react";
import Image from "next/image";
import Link from "next/link";
import { ThemeColors } from "@/components/ThemeColors";
import Spinner from "@/components/Spinner";
import client from "@/lib/client";

const iconMap = {
  assignment_due: "/assets/notifications/assignment-due.svg",
  graded: "/assets/notifications/assignment-graded.svg",
  reply: "/assets/notifications/reply-icon.svg",
  lesson: "/assets/notifications/new-lesson-icon.svg",
  team: "/assets/notifications/team-update-icon.svg",
  certificate: "/assets/notifications/trophy-icon.svg",
  System: "/assets/notifications/team-update-icon.svg",
};

function formatTime(dateString) {
  const date = new Date(dateString);
  const now = new Date("2026-03-31T15:00:00Z"); // Using current approximate time as base
  const diffInHours = Math.floor((now - date) / (1000 * 60 * 60));

  if (diffInHours < 1) return "Just now";
  if (diffInHours < 24)
    return `${diffInHours} hour${diffInHours > 1 ? "s" : ""} ago`;

  const diffInDays = Math.floor(diffInHours / 24);
  return `${diffInDays} day${diffInDays > 1 ? "s" : ""} ago`;
}

function NotificationItem({ notif, onMarkRead }) {
  return (
    <Link
      href={notif.actionUrl || "#"}
      onClick={() => {
        if (!notif.isRead) onMarkRead(notif.id);
      }}
      className={`relative flex items-center px-6 py-0 border-b transition-all duration-200 cursor-pointer group hover:bg-black/20 no-underline ${notif.isRead ? "bg-transparent" : "bg-[#131d2e]"}`}
      style={{
        height: "117px",
        borderColor: "#D6E3F5",
        borderBottomWidth: "0.5px",
      }}
    >
      {/* Green tint overlay for new notifications */}
      {!notif.isRead && (
        <div
          className="absolute inset-0 pointer-events-none"
          style={{ background: "#06580D", opacity: 0.1 }}
        />
      )}

      {/* Content row */}
      <div className="relative flex items-center gap-6 flex-1 min-w-0">
        {/* Icon box */}
        <div
          className="shrink-0 flex items-center justify-center rounded-lg"
          style={{ width: 32, height: 32, background: "#314568" }}
        >
          <Image
            src={iconMap[notif.type] || iconMap["System"]}
            alt={notif.type}
            width={20}
            height={20}
            className="w-auto h-auto"
          />
        </div>

        {/* Text */}
        <div className="flex flex-col gap-2 min-w-0 flex-1">
          <span
            className="font-bold text-white truncate"
            style={{
              fontSize: 16,
              lineHeight: "125%",
              fontFamily: "'DM Sans', sans-serif",
            }}
          >
            {notif.title}
          </span>
          <span
            className="truncate md:whitespace-normal"
            style={{
              fontSize: 16,
              lineHeight: "125%",
              color: "#7D7F82",
              fontFamily: "'DM Sans', sans-serif",
            }}
          >
            {notif.message}
          </span>
          <span
            style={{
              fontSize: 12,
              lineHeight: "125%",
              color: "#7D7F82",
              fontFamily: "'DM Sans', sans-serif",
            }}
          >
            {formatTime(notif.createdAt)}
          </span>
        </div>
      </div>

      {/* Green dot for unread */}
      {!notif.isRead && (
        <div
          className="relative shrink-0 ml-4"
          style={{ width: 18, height: 18 }}
        >
          {/* Outer ring */}
          <div
            className="absolute inset-0 rounded-full border"
            style={{ borderColor: "rgba(0,128,0,0.3)" }}
          />
          {/* Inner dot */}
          <div
            className="absolute rounded-full"
            style={{
              width: 10,
              height: 10,
              background: "#008000",
              top: "50%",
              left: "50%",
              transform: "translate(-50%, -50%)",
            }}
          />
        </div>
      )}
    </Link>
  );
}

async function fetchNotifications() {
  const token = localStorage.getItem("token");

  const response = await fetch(`${client.defaults.baseURL}/Notification`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      ...(token && { Authorization: `Bearer ${token}` }),
    },
  });

  if (!response.ok) {
    throw new Error(`Failed to fetch notifications (${response.status})`);
  }

  const data = await response.json();
  return data.data ?? data;
}

async function fetchUnreadNotificationsCount() {
  const token = localStorage.getItem("token");

  const response = await fetch(
    `${client.defaults.baseURL}/Notification/unread-count`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        ...(token && { Authorization: `Bearer ${token}` }),
      },
    },
  );

  if (!response.ok) {
    throw new Error(`Failed to fetch unread count (${response.status})`);
  }

  const data = await response.json();
  return data.data ?? data;
}

async function markAllNotificationsRead() {
  const token = localStorage.getItem("token");

  const response = await fetch(
    `${client.defaults.baseURL}/Notification/read-all`,
    {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        ...(token && { Authorization: `Bearer ${token}` }),
      },
    },
  );

  if (!response.ok) {
    throw new Error(
      `Failed to mark notifications as read (${response.status})`,
    );
  }

  return response.json();
}

async function markNotificationRead(notificationId) {
  const token = localStorage.getItem("token");

  const response = await fetch(
    `${client.defaults.baseURL}/Notification/${notificationId}/read`,
    {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        ...(token && { Authorization: `Bearer ${token}` }),
      },
    },
  );

  if (!response.ok) {
    throw new Error(`Failed to mark notification as read (${response.status})`);
  }

  return response.json();
}
export default function NotificationsPage() {
  const [notificationList, setNotificationList] = useState([]);
  const [unreadNotificationCount, setUnreadNotificationCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  function loadNotifications() {
    setLoading(true);
    setError(null);

    fetchNotifications()
      .then(setNotificationList)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));

    fetchUnreadNotificationsCount()
      .then(setUnreadNotificationCount)
      .catch(console.error);
  }

  useEffect(() => {
    loadNotifications();
  }, []);

  const newCount = notificationList.filter((n) => !n.isRead).length;

  function handleMarkRead(notificationId) {
    setNotificationList((prev) =>
      prev.map((n) => (n.id === notificationId ? { ...n, isRead: true } : n)),
    );
    setUnreadNotificationCount((prev) => Math.max(0, prev - 1));
    markNotificationRead(notificationId).catch(console.error);
  }

  return (
    <main
      className=""
      style={{ fontFamily: "var(--font-inter), 'Inter', sans-serif" }}
    >
      <div
        className="min-h-screen w-full"
        style={{ backgroundColor: ThemeColors.bgBlue }}
      >
        {/* Main container — max 1436px, full width on tablet */}
        <div
          className="mx-auto w-full"
          style={{
            maxWidth: 1436,
            borderLeft: "0.75px solid #7D7F82",
            borderRight: "0.75px solid #7D7F82",
          }}
        >
          {/* ── Header ── */}
          <div
            className="flex flex-row items-center justify-between px-5 sm:px-6"
            style={{
              height: 98,
              background: "transparent",
              borderBottom: "0.75px solid #D6E3F5",
            }}
          >
            <h1
              className="font-bold text-white"
              style={{ fontSize: "clamp(24px, 3vw, 40px)", lineHeight: "125%" }}
            >
              Notifications
            </h1>

            {/* Mark all read button — hidden while loading or when empty */}
            {!loading && notificationList.length > 0 && (
              <div className="relative">
                <button
                  onClick={async () => {
                    try {
                      await markAllNotificationsRead();
                      setNotificationList((prev) =>
                        prev.map((n) => ({ ...n, isRead: true })),
                      );
                      setUnreadNotificationCount(0);
                    } catch (err) {
                      setError(err.message);
                    }
                  }}
                  className="flex items-center justify-center font-bold text-white rounded-lg transition-colors duration-150 hover:bg-[#5a5b5d] cursor-pointer"
                  style={{
                    padding: "0 16px",
                    height: 40,
                    minWidth: 120,
                    background: "#4B4C4E",
                    border: "1px solid #D6E3F5",
                    borderRadius: 8,
                    fontSize: 14,
                    whiteSpace: "nowrap",
                  }}
                >
                  Mark all read
                </button>

                {unreadNotificationCount > 0 && (
                  <span
                    className="absolute -top-2 -right-2 flex items-center justify-center rounded-full bg-[#0950C3] text-white text-xs font-bold"
                    style={{ width: 22, height: 22, fontSize: 11 }}
                  >
                    {unreadNotificationCount}
                  </span>
                )}
              </div>
            )}
          </div>

          {/* ── Notification list ── */}
          <div className="flex flex-col" style={{ borderLeft: "none" }}>
            {loading && <Spinner message="Loading notifications..." />}

            {error && (
              <div className="flex flex-col items-center justify-center min-h-[calc(100vh-98px)] gap-4">
                <span className="text-[#7D7F82] text-base font-medium">
                  Something went wrong while loading your notifications.
                </span>
                <button
                  onClick={loadNotifications}
                  className="flex items-center justify-center font-bold text-white rounded-lg transition-colors duration-150 hover:bg-[#0950C3]/80 cursor-pointer"
                  style={{
                    padding: "0 20px",
                    height: 40,
                    background: "#0950C3",
                    borderRadius: 8,
                    fontSize: 14,
                    border: "none",
                  }}
                >
                  Retry
                </button>
              </div>
            )}

            {!loading && !error && notificationList.length === 0 && (
              <div className="flex flex-col items-center justify-center py-24 gap-4">
                <Image
                  src="/assets/notifications/notification_icon.svg"
                  alt="No notifications"
                  width={48}
                  height={48}
                  className="opacity-40"
                />
                <span className="text-[#7D7F82] text-base font-medium">
                  You're all caught up — no notifications right now.
                </span>
              </div>
            )}

            {!loading &&
              !error &&
              notificationList.map((notif) => (
                <NotificationItem
                  key={notif.id}
                  notif={notif}
                  onMarkRead={handleMarkRead}
                />
              ))}
          </div>
        </div>
      </div>
    </main>
  );
}
