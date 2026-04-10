"use client";
import { useState } from "react";
import Link from "next/link";
import { ThemeColors } from "@/components/ThemeColors";

export default function LessonPlayer() {
  const [note, setNote] = useState("");

  const lessons = [
    { id: 1, title: "Intro to UX Design", duration: "35m", completed: true },
    { id: 2, title: "User Research", duration: "45m", completed: true },
    { id: 3, title: "Wireframing", duration: "48m", active: true },
    { id: 4, title: "Design Systems", duration: "40m" },
    { id: 5, title: "Usability Testing", duration: "55m" },
    { id: 6, title: "Color Theory", duration: "30m" },
  ];

  return (
    <div className="flex flex-col h-full overflow-hidden">
      {/* Top Header Bar */}
      <header className="flex items-center justify-between px-6 py-3 border-b border-zinc-800 bg-[#0d1117] shrink-0">
        <div className="flex items-center gap-4">
          <Link href="/coursecatalog" className="px-3 py-1 bg-zinc-800 rounded text-xs text-zinc-400 hover:text-white transition">
            Back
          </Link>
          <div className="flex flex-col">
            <h1 className="text-white font-bold text-sm">UI/UX Fundamentals</h1>
            <p className="text-zinc-500 text-xs">Lesson 3: Wireframing & Prototyping</p>
          </div>
        </div>

        <div className="flex items-center gap-6">
          <div className="flex items-center gap-3">
            <div className="w-32 h-1.5 bg-zinc-800 rounded-full overflow-hidden">
              <div className="h-full bg-blue-500" style={{ width: '72%' }}></div>
            </div>
            <span className="text-xs text-blue-400 font-medium">72%</span>
          </div>
          <button className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-1.5 rounded-lg text-sm font-bold transition">
            Mark Complete
          </button>
        </div>
      </header>

      {/* Main Content Grid */}
      <div className="flex-1 grid grid-cols-1 lg:grid-cols-[1fr_300px] overflow-hidden">
        
        {/* LEFT: Video & Details - Optimized for Compactness */}
        <div className="overflow-y-auto p-4 md:p-6 space-y-4 md:space-y-6 custom-scrollbar">
          {/* Video Player Placeholder - Shortened Height */}
          <div className="relative aspect-21/5 bg-sky-200 rounded-2xl border border-zinc-800 flex items-center justify-center overflow-hidden group">
            <div className="w-14 h-14 bg-blue-600/90 rounded-full flex items-center justify-center cursor-pointer group-hover:scale-110 transition-transform">
               <span className="text-white ml-1 text-xl">▶</span>
            </div>
            {/* Bottom Controls Bar Mockup */}
            <div className="absolute bottom-0 left-0 right-0 h-1.5 bg-zinc-700">
               <div className="h-full bg-blue-500 w-[40%] rounded-r-full"></div>
            </div>
          </div>

          {/* Compact Description Card */}
          <section className="bg-[#161b22] border border-zinc-800 rounded-2xl p-5">
            <h3 className="text-white font-bold mb-2 text-sm">Wireframing & Prototyping</h3>
            <p className="text-zinc-400 text-xs leading-relaxed mb-5 max-w-4xl">
              In this lesson, we&apos;ll explore the core principles of wireframing, translating user research insights into tangible, testable layouts. 
              You&apos;ll learn low-fidelity and high-fidelity wireframes, when to use each, and how to prototype interactions using Figma.
            </p>
            <div className="flex gap-2">
              {['Figma', 'Wireframes', 'Prototyping'].map(tag => (
                <span key={tag} className="px-3 py-1 bg-blue-900/30 text-blue-400 text-[10px] font-medium rounded-full border border-blue-800/50">
                  {tag}
                </span>
              ))}
            </div>
          </section>

          {/* Compact Resources Card */}
          <section className="bg-[#161b22] border border-zinc-800 rounded-2xl p-5">
            <h3 className="text-white font-bold mb-3 flex items-center gap-2 text-xs">
              <span>🔗</span> Resources
            </h3>
            <div className="space-y-2">
              <div className="flex items-center justify-between p-2.5 border border-zinc-800 rounded-xl hover:bg-zinc-800/50 transition cursor-pointer">
                <span className="text-zinc-300 text-[11px] flex items-center gap-2">📄 Wireframing checklist.pdf</span>
                <span className="text-blue-500 text-[11px] font-bold">Download</span>
              </div>
              <div className="flex items-center justify-between p-2.5 border border-zinc-800 rounded-xl hover:bg-zinc-800/50 transition cursor-pointer">
                <span className="text-zinc-300 text-[11px] flex items-center gap-2">🎨 Figma Starter Template</span>
                <span className="text-blue-500 text-[11px] font-bold">Open</span>
              </div>
            </div>
          </section>
        </div>

        {/* RIGHT: Playlist & Notes - Remained compact but efficient */}
        <div className="border-l border-zinc-800 bg-[#0d1117] flex flex-col overflow-hidden shrink-0">
          {/* Playlist */}
          <div className="p-4 flex-1 overflow-y-auto">
            <h3 className="text-white font-bold text-sm mb-4">Course Lessons</h3>
            <div className="space-y-2">
              {lessons.map((l) => (
                <div 
                  key={l.id} 
                  className={`p-2.5 rounded-xl flex items-center justify-between cursor-pointer transition ${
                    l.active ? 'bg-blue-600/20 border border-blue-500/50' : 'hover:bg-zinc-800'
                  }`}
                >
                  <div className="flex items-center gap-2.5">
                    <div className={`w-5 h-5 rounded flex items-center justify-center text-[10px] ${
                      l.completed ? 'bg-blue-600 text-white' : l.active ? 'bg-blue-500 text-white' : 'border border-zinc-700 text-zinc-600'
                    }`}>
                      {l.completed ? '✓' : l.id}
                    </div>
                    <p className={`text-[11px] font-medium ${l.active ? 'text-white' : 'text-zinc-400'}`}>{l.title}</p>
                  </div>
                  <span className="text-[10px] text-zinc-500">{l.duration}</span>
                </div>
              ))}
            </div>
          </div>

          {/* Quick Notes */}
          <div className="p-4 border-t border-zinc-800 bg-[#161b22]">
            <h3 className="text-white font-bold text-sm mb-3">Quick Notes</h3>
            <textarea 
              value={note}
              onChange={(e) => setNote(e.target.value)}
              placeholder="Jot down notes for this lesson..."
              className="w-full h-20 bg-[#0d1117] border border-zinc-800 rounded-lg p-3 text-[11px] text-zinc-300 focus:outline-none focus:border-blue-500 resize-none"
            />
            <button className="w-full mt-3 bg-zinc-700 hover:bg-zinc-600 text-white py-2 rounded-lg text-xs font-bold transition">
              Save Note
            </button>
          </div>
        </div>

      </div>
    </div>
  );
}