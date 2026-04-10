'use client';

import React, { useEffect, useState } from 'react';
import client from '@/lib/client';

const defaultProfile = {
  id: '1',
  fullName: 'Adeeze Okoro',
  email: 'adeeze@learntlm.org',
  phoneNumber: '+2342324456789',
  userId: 'TMI-2025-047',
  role: 'UI/UX Design Intern',
  location: 'Cohort 3 - Lagos, Nigeria',
  tags: ['UI/UX Design', 'Week 6', 'Top Contributor'],
  courses: 4,
  avgProgress: 68,
  certificates: 2,
  avgScore: 88,
  achievements: [
    {
      id: '1',
      icon: '🔥',
      title: '7-Day Streak',
      description: 'Logged in 7 days in a row',
    },
    {
      id: '2',
      icon: '⚪',
      title: 'Top Contributor',
      description: '31 discussion posts',
    },
    {
      id: '3',
      icon: '⭐',
      title: 'High Scorer',
      description: 'Achieved top scores in assessments',
    },
  ],
};

export default function ProfilePage() {
  const [profile, setProfile] = useState(defaultProfile);
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({
    fullName: profile.fullName,
    email: profile.email,
    phoneNumber: profile.phoneNumber,
  });
  const [loading, setLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [message, setMessage] = useState('');

  useEffect(() => {
    const hydrateProfile = async () => {
      try {
        const response = await client.get('/Profile/me');
        const payload = response.data?.data;

        if (!payload) {
          return;
        }

        const personal = payload.personalInformation || {};
        const summary = payload.learningSummary || {};
        const tags = Array.isArray(payload.badges) && payload.badges.length
          ? payload.badges.map((badge) => badge.label)
          : [personal.discipline || 'Learner'];

        const achievements = Array.isArray(payload.achievements) && payload.achievements.length
          ? payload.achievements.map((item, index) => ({
              id: String(index + 1),
              icon: index % 3 === 0 ? '🔥' : index % 3 === 1 ? '⭐' : '🏅',
              title: item.title,
              description: item.description,
            }))
          : defaultProfile.achievements;

        const nextProfile = {
          id: String(payload.userId || '1'),
          fullName: payload.fullName || defaultProfile.fullName,
          email: personal.email || defaultProfile.email,
          phoneNumber: personal.phoneNumber || '',
          userId: payload.publicId || defaultProfile.userId,
          role: payload.headline || personal.discipline || defaultProfile.role,
          location: personal.location || personal.cohortLabel || defaultProfile.location,
          tags,
          courses: summary.courses ?? defaultProfile.courses,
          avgProgress: Math.round(summary.averageProgress ?? defaultProfile.avgProgress),
          certificates: summary.certificates ?? defaultProfile.certificates,
          avgScore: Number(summary.averageScore ?? defaultProfile.avgScore),
          achievements,
        };

        setProfile(nextProfile);
        setFormData({
          fullName: nextProfile.fullName,
          email: nextProfile.email,
          phoneNumber: nextProfile.phoneNumber,
        });
      } catch (error) {
        console.error('Error loading profile:', error);
        setMessage('Unable to load profile from server. Showing saved layout values.');
      } finally {
        setLoading(false);
      }
    };

    hydrateProfile();
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSaveChanges = async () => {
    setIsSaving(true);
    setMessage('');
    try {
      const trimmed = formData.fullName.trim();
      const parts = trimmed.split(/\s+/).filter(Boolean);
      const firstName = parts[0] || '';
      const lastName = parts.slice(1).join(' ') || firstName;

      const response = await client.put('/Profile/me', {
        firstName,
        lastName,
        phoneNumber: formData.phoneNumber || null,
      });

      const payload = response.data?.data;
      const personal = payload?.personalInformation || {};

      setProfile((prev) => ({
        ...prev,
        fullName: payload?.fullName || formData.fullName,
        email: personal.email || prev.email,
        phoneNumber: personal.phoneNumber || formData.phoneNumber,
      }));

      setFormData((prev) => ({
        ...prev,
        fullName: payload?.fullName || prev.fullName,
        email: personal.email || prev.email,
        phoneNumber: personal.phoneNumber || prev.phoneNumber,
      }));

      setIsEditing(false);
      setMessage('Profile updated successfully.');
    } catch (error) {
      console.error('Error saving profile:', error);
      setMessage('Failed to save profile. Please try again.');
    } finally {
      setIsSaving(false);
    }
  };

  const handleEditToggle = () => {
    if (isEditing) {
      setFormData({
        fullName: profile.fullName,
        email: profile.email,
        phoneNumber: profile.phoneNumber,
      });
    }
    setIsEditing(!isEditing);
  };

  const getInitials = (name) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  return (
    <div className="min-h-screen w-full bg-[#101723] text-[#FAFCFF]">
      <div className="mx-auto w-full max-w-screen-2xl px-5 pb-8 pt-6 lg:px-10">
        <div className="mb-10 flex items-center justify-between border-b border-[#4B4C4E] pb-4">
          <h1 className="text-[40px] font-bold leading-tight">My Profile</h1>
          <button
            className="rounded-2xl border border-[#D6E3F5]/50 px-6 py-3 text-xl font-bold text-[#0950C3] transition hover:bg-[#D6E3F5]/10"
            onClick={handleEditToggle}
            aria-label="Edit profile"
          >
            {isEditing ? 'Cancel Edit' : 'Edit Profile'}
          </button>
        </div>

        {message && (
          <div className="mb-4 rounded-lg border border-[#4B4C4E] bg-[#0D1522] px-4 py-3 text-sm text-[#CEE0FD]">
            {message}
          </div>
        )}

        {loading && (
          <div className="mb-4 rounded-lg border border-[#4B4C4E] bg-[#0D1522] px-4 py-3 text-sm text-[#CEE0FD]">
            Loading profile...
          </div>
        )}

        <div className="mb-10">
          <div className="rounded-2xl border border-[#D6E3F5] bg-[#101723] p-6">
            <div className="flex flex-col gap-6 md:flex-row md:items-center">
              <div className="flex h-44 w-44 items-center justify-center rounded-3xl bg-[#0950C3] text-5xl font-bold text-white">
                {getInitials(profile.fullName)}
              </div>
              <div>
                <h1 className="text-[40px] font-bold leading-tight">{profile.fullName}</h1>
                <p className="text-[33px] font-normal leading-tight text-[#0950C3]">{profile.userId}</p>
                <p className="text-[28px] font-normal leading-tight text-[#D6E3F5]">
                  {profile.role} - {profile.location}
                </p>
                <div className="mt-4 flex flex-wrap gap-3">
                  {profile.tags.map((tag, index) => (
                    <span
                      key={index}
                      className="rounded-2xl bg-[#CEE0FD] px-4 py-2 text-base font-bold text-[#0950C3]"
                    >
                      {tag}
                    </span>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="grid gap-6 xl:grid-cols-[615px_minmax(0,1fr)]">
          <div>
            <div className="rounded-2xl border border-[#D6E3F5] bg-[#101723] p-6">
              <h2 className="mb-6 text-[33px] font-bold leading-tight">Personal Information</h2>

              <div className="mb-4">
                <label htmlFor="fullName" className="mb-1 block text-base font-bold text-[#D6E3F5]">
                  Full Name
                </label>
                <input
                  id="fullName"
                  name="fullName"
                  type="text"
                  className="h-11 w-full rounded-lg border border-[#D6E3F5] bg-transparent px-3 text-sm text-[#FAFCFF] outline-none focus:border-[#0950C3] disabled:opacity-70"
                  value={formData.fullName}
                  onChange={handleInputChange}
                  disabled={!isEditing}
                />
              </div>

              <div className="mb-4">
                <label htmlFor="email" className="mb-1 block text-base font-bold text-[#D6E3F5]">
                  Email
                </label>
                <input
                  id="email"
                  name="email"
                  type="email"
                  className="h-11 w-full rounded-lg border border-[#D6E3F5] bg-transparent px-3 text-sm text-[#FAFCFF] outline-none focus:border-[#0950C3] disabled:opacity-70"
                  value={formData.email}
                  onChange={handleInputChange}
                  disabled={!isEditing}
                />
              </div>

              <div className="mb-4">
                <label htmlFor="phoneNumber" className="mb-1 block text-base font-bold text-[#D6E3F5]">
                  Phone Number
                </label>
                <input
                  id="phoneNumber"
                  name="phoneNumber"
                  type="tel"
                  className="h-11 w-full rounded-lg border border-[#D6E3F5] bg-transparent px-3 text-sm text-[#FAFCFF] outline-none focus:border-[#0950C3] disabled:opacity-70"
                  value={formData.phoneNumber}
                  onChange={handleInputChange}
                  disabled={!isEditing}
                />
              </div>

              {isEditing && (
                <div className="mt-6 flex flex-col gap-3 border-t border-[#D6E3F5]/30 pt-4 sm:flex-row">
                  <button
                    className="rounded-lg bg-[#0950C3] px-4 py-2 text-base font-bold text-white transition hover:bg-[#0a61e9] disabled:opacity-60"
                    onClick={handleSaveChanges}
                    disabled={isSaving}
                  >
                    {isSaving ? 'Saving...' : 'Save Changes'}
                  </button>
                  <button
                    className="rounded-lg border border-[#D6E3F5] bg-transparent px-4 py-2 text-base font-bold text-[#D6E3F5] transition hover:bg-[#D6E3F5]/10 disabled:opacity-60"
                    onClick={handleEditToggle}
                    disabled={isSaving}
                  >
                    Cancel
                  </button>
                </div>
              )}
            </div>
          </div>

          <div className="space-y-6">
            <div className="rounded-2xl border border-[#D6E3F5] bg-[#101723] p-6">
              <h2 className="mb-5 text-[33px] font-bold leading-tight">Learning Summary</h2>
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="rounded-2xl border border-[#D6E3F5] p-6 text-center">
                  <div className="text-[33px] font-bold text-[#09C398]">1</div>
                  <div className="text-[23px] text-white">Course(s)</div>
                </div>
                <div className="rounded-2xl border border-[#D6E3F5] p-6 text-center">
                  <div className="text-[33px] font-bold text-[#0950C3]">{profile.avgProgress}%</div>
                  <div className="text-[23px] text-white">Avg Progress</div>
                </div>
                <div className="rounded-2xl border border-[#D6E3F5] p-6 text-center">
                  <div className="text-[33px] font-bold text-[#DE55F3]">{profile.certificates}</div>
                  <div className="text-[23px] text-white">Certificates</div>
                </div>
                <div className="rounded-2xl border border-[#D6E3F5] p-6 text-center">
                  <div className="text-[33px] font-bold text-[#EF9B15]">{profile.avgScore}</div>
                  <div className="text-[23px] text-white">Avg Score</div>
                </div>
              </div>
            </div>

            <div className="rounded-2xl border border-[#D6E3F5] bg-[#101723] p-6">
              <h2 className="mb-5 text-[33px] font-bold leading-tight">Achievements</h2>
              <div className="space-y-4">
                {profile.achievements.map((achievement) => (
                  <div
                    key={achievement.id}
                    className="flex items-start gap-4 rounded-2xl border border-[#D6E3F5] bg-[#101723] px-4 py-5"
                  >
                    <span
                      className="text-2xl"
                      role="img"
                      aria-label={achievement.title}
                    >
                      {achievement.icon}
                    </span>
                    <div>
                      <div className="text-[28px] font-bold leading-tight">
                        {achievement.title}
                      </div>
                      <div className="text-[19px] text-[#ADC7EB]">
                        {achievement.description}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
