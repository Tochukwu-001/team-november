"use client";

import { useEffect, useMemo, useState } from 'react';
import client from '@/lib/client';

const fallbackCards = [
  { title: 'UI/UX Fundamentals', value: 72, color: '#00D4AA' },
  { title: 'Product Thinking', value: 45, color: '#0950C3' },
  { title: 'Agile & Scrum', value: 12, color: '#8B61E8' },
  { title: 'Overall Progress', value: 70, color: '#EF9807' },
];

const fallbackSkills = [
  { name: 'Visual Design', value: 82 },
  { name: 'User Research', value: 68 },
  { name: 'Prototyping', value: 55 },
  { name: 'Collaboration', value: 90 },
  { name: 'Communication', value: 75 },
];

const fallbackActivities = [
  { title: 'Sprint Retrospective', subtitle: 'Agile & Scrum - Mar 15', score: 88, color: '#0950C3' },
  { title: 'User Research Report', subtitle: 'Product Thinking - Mar 10', score: 92, color: '#00D4AA' },
  { title: 'Color Palette Project', subtitle: 'UI/UX Fundamentals - Mar 5', score: 79, color: '#8B61E8' },
];

export default function ProgressPage() {
  const [cards, setCards] = useState(fallbackCards);
  const [skills, setSkills] = useState(fallbackSkills);
  const [activities, setActivities] = useState(fallbackActivities);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadProgress = async () => {
      try {
        const response = await client.get('/Progress/me');
        const payload = response.data?.data;

        if (!payload) {
          return;
        }

        const palette = ['#00D4AA', '#0950C3', '#8B61E8'];
        const courseCards = (payload.courseProgressCards || []).slice(0, 3).map((card, index) => ({
          title: card.courseTitle,
          value: Math.round(card.percentage || 0),
          color: palette[index % palette.length],
        }));

        const overall = {
          title: 'Overall Progress',
          value: Math.round(payload.overallProgressPercentage || 0),
          color: '#EF9807',
        };

        setCards((courseCards.length ? [...courseCards, overall] : fallbackCards).slice(0, 4));

        const apiSkills = (payload.skillBreakdown || []).map((item) => ({
          name: item.skill,
          value: Math.round(item.score || 0),
        }));
        if (apiSkills.length) {
          setSkills(apiSkills);
        }

        const apiActivities = (payload.gradedWork || []).slice(0, 4).map((item, index) => {
          const activityDate = item.activityDate ? new Date(item.activityDate) : null;
          const dateLabel = activityDate
            ? activityDate.toLocaleDateString(undefined, { month: 'short', day: 'numeric' })
            : 'Recent';

          return {
            title: item.title,
            subtitle: `${item.courseTitle} - ${dateLabel}`,
            score: Number(item.score || 0),
            color: palette[index % palette.length],
          };
        });

        if (apiActivities.length) {
          setActivities(apiActivities);
        }
      } catch (err) {
        console.error('Progress fetch error:', err);
        setError('Unable to load progress right now. Showing current layout data.');
      } finally {
        setLoading(false);
      }
    };

    loadProgress();
  }, []);

  const visibleCards = useMemo(() => {
    if (cards.length === 4) {
      return cards;
    }

    return [...cards, ...fallbackCards].slice(0, 4);
  }, [cards]);

  return (
    <div className="min-h-screen bg-[#101723] px-5 pb-8 text-[#FAFCFF] lg:px-9">
      <header className="flex h-26 items-center border-b border-[#4B4C4E] pl-2">
        <h1 className="text-[40px] font-bold leading-tight">My Progress</h1>
      </header>

      {loading && (
        <div className="mt-4 rounded-lg border border-[#7D7F82] bg-[#101723] px-4 py-3 text-sm text-[#CEE0FD]">
          Loading progress...
        </div>
      )}

      {error && (
        <div className="mt-4 rounded-lg border border-[#7D7F82] bg-[#101723] px-4 py-3 text-sm text-[#CEE0FD]">
          {error}
        </div>
      )}

      <section className="mt-10 grid gap-6 xl:grid-cols-4 md:grid-cols-2 grid-cols-1">
        {visibleCards.map((card) => (
          <article
            key={card.title}
            className="relative flex min-h-52 flex-col items-center justify-center rounded-2xl border border-[#D6E3F5] bg-[#101723] px-4 py-8"
          >
            <div
              className="grid h-24 w-24 place-items-center rounded-full"
              style={{ background: `conic-gradient(${card.color} ${card.value}%, #4d4e50 ${card.value}% 100%)` }}
            >
              <div className="grid h-20 w-20 place-items-center rounded-full bg-[#101723] text-[23px] font-bold">
                <span style={{ color: card.color }}>{card.value}%</span>
              </div>
            </div>
            <p className="mt-3 text-center text-base text-[#D6E3F5]">{card.title}</p>
            <span className="absolute -right-5 -top-7 h-24 w-20 rounded-full bg-[#415C8B]/50" />
          </article>
        ))}
      </section>

      <section className="mt-16 grid gap-16 xl:grid-cols-2 grid-cols-1">
        <article className="rounded-xl border border-[#7D7F82] bg-[#101723] px-6 pb-8 pt-6">
          <h2 className="text-[33px] font-bold leading-tight">Skill Breakdown</h2>
          <div className="mt-7 space-y-4">
            {skills.map((skill) => (
              <div key={skill.name} className="grid items-center gap-4 lg:grid-cols-[180px_1fr_48px] grid-cols-1">
                <p className="text-[23px] leading-tight text-[#D6E3F5]">{skill.name}</p>
                <div className="h-3.5 overflow-hidden rounded-xl bg-[#7D7F82]/50">
                  <div
                    className="h-full rounded-xl bg-[linear-gradient(279.23deg,#ADC7EB_7.56%,#627185_156.44%)]"
                    style={{ width: `${Math.max(0, Math.min(100, skill.value))}%` }}
                    aria-hidden="true"
                  />
                </div>
                <p className="text-base font-bold text-[#0950C3]">{skill.value}%</p>
              </div>
            ))}
          </div>
        </article>

        <article className="rounded-xl border border-[#7D7F82] bg-[#101723] p-6">
          <h2 className="sr-only">Recent Scores</h2>
          <div className="space-y-7">
            {activities.map((activity) => (
              <div
                key={activity.title}
                className="flex items-center justify-between rounded-[10px] bg-[#1A2234] px-7 py-2"
              >
                <div>
                  <p className="text-[23px] font-bold leading-tight">{activity.title}</p>
                  <p className="text-base font-bold text-[#CEE0FD]">{activity.subtitle}</p>
                </div>
                <p className="text-[23px] font-bold" style={{ color: activity.color }}>
                  {activity.score}
                </p>
              </div>
            ))}
          </div>
        </article>
      </section>
    </div>
  );
}
