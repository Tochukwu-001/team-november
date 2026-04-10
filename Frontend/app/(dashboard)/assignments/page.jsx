"use client";
import { useState, useEffect } from "react";
import { ThemeColors } from '@/components/ThemeColors';
import client from "@/lib/client";

export default function Assignments() {
  const [assignments, setAssignments] = useState([]);
  const [filteredAssignments, setFilteredAssignments] = useState([]);
  const [activeCategory, setActiveCategory] = useState('All');
  const [loading, setLoading] = useState(true);

  const categories = ['All', 'Pending', 'Submitted', 'Graded'];

  useEffect(() => {
    const fetchAssignments = async () => {
      try {
        setLoading(true);
        // Using the endpoint from your Swagger image
        const response = await client.get('/Assignment/my');
        const data = response.data?.data || [];
        setAssignments(data);
        setFilteredAssignments(data);
      } catch (err) {
        console.error("Error fetching assignments:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchAssignments();
  }, []);

  // Filter logic whenever activeCategory or assignments change
  useEffect(() => {
    if (activeCategory === 'All') {
      setFilteredAssignments(assignments);
    } else {
      setFilteredAssignments(
        assignments.filter(asn => asn.status === activeCategory)
      );
    }
  }, [activeCategory, assignments]);

  const getStatusStyles = (status) => {
    switch (status) {
      case 'Pending':
        return "bg-stone-800 text-orange-400 border-none";
      case 'Submitted':
        return "bg-blue-100 text-blue-800 border-none";
      case 'Graded':
        return "bg-blue-600/30 text-blue-400 border border-blue-500/40";
      default:
        return "bg-zinc-800 text-zinc-400";
    }
  };

  return (
    <main style={{ backgroundColor: ThemeColors.bgBlue }} className="min-h-screen text-zinc-300 font-sans p-4 md:p-8 overflow-y-auto">
      
      {/* Top Navigation */}
      <div className="flex max-md:flex-col max-md:gap-3 md:items-center justify-between mb-10">
        <h1 className="text-2xl font-bold text-white">Assignments</h1>
        <div className="flex items-center gap-6">
          {categories.map((cat) => (
            <button 
              key={cat} 
              onClick={() => setActiveCategory(cat)}
              className={`text-sm font-bold transition-all ${activeCategory === cat ? 'text-white border-b-2 border-blue-500 pb-1' : 'text-zinc-400 hover:text-zinc-200'}`}
            >
              {cat}
            </button>
          ))}
        </div>
      </div>

      <div className="space-y-6">
        {loading ? (
          <p className="text-center py-20 text-zinc-500 italic">Loading your assignments...</p>
        ) : filteredAssignments.length > 0 ? (
          filteredAssignments.map((asn) => (
            <section key={asn.id} className="border border-slate-400 rounded-2xl p-6 bg-transparent w-full">
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h2 className="text-lg font-bold text-white">{asn.title}</h2>
                  <div className="flex gap-2 mt-2">
                    <span className="bg-red-500/20 text-red-400 text-[10px] font-bold px-3 py-1 rounded-full border border-red-500/30 text-center">
                      Due: {new Date(asn.dueDate).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}
                    </span>
                    <span className="bg-blue-600 text-white text-[10px] font-bold px-3 py-1 rounded-full text-center">
                      {asn.courseTitle}
                    </span>
                  </div>
                </div>
                <span className={`text-[10px] font-bold px-4 py-1.5 rounded-full flex items-center gap-2 ${getStatusStyles(asn.status)}`}>
                  {asn.status === 'Pending' && <span className="text-xs">→</span>}
                  {asn.status} {asn.grade ? `- ${asn.grade}/100` : ''}
                </span>
              </div>
              
              <p className="text-[11px] text-zinc-500 mb-6">
                {asn.description}
              </p>

              {/* Conditional View based on status */}
              {asn.status === 'Pending' ? (
                <>
                  <div className="border-2 border-dashed border-zinc-800 rounded-2xl py-12 flex flex-col items-center justify-center bg-black/5 mb-4">
                     <span className="text-2xl mb-2">☁️</span>
                     <p className="text-[10px] text-zinc-400">
                       <button className="text-blue-500 underline mr-1">Click to upload</button> or drag and drop
                     </p>
                     <p className="text-[9px] text-zinc-600 mt-1">PDF, Figma link, PNG, ZIP (max 50MB)</p>
                  </div>
                  <div className="flex gap-2">
                    <input 
                      type="text" 
                      placeholder="Or paste Figma link here..." 
                      className="flex-1 bg-black/20 border border-zinc-800 rounded-lg px-4 py-2.5 text-xs text-zinc-400 outline-none focus:border-blue-500"
                    />
                    <button className="bg-blue-600 text-white text-xs font-bold px-6 py-2 rounded-lg">Submit</button>
                  </div>
                </>
              ) : (
                <div className="space-y-3">
                  <div className="border border-zinc-800 rounded-xl p-4 bg-black/10">
                    <p className="text-[10px] font-bold text-white mb-1 flex items-center gap-2">📄 Submission Details</p>
                    <p className="text-[10px] text-zinc-500 ml-6">
                      {asn.submissionFile || "Link submitted"} on {new Date(asn.submittedAt).toLocaleDateString()}
                    </p>
                  </div>
                  {asn.status === 'Graded' ? (
                     <div className="border border-blue-900/30 rounded-xl p-4 bg-blue-900/5">
                        <p className="text-[11px] font-bold text-green-500 flex items-center gap-2 mb-1">
                          <span className="bg-green-500 text-white text-[8px] p-0.5 rounded">✓</span> Grade: {asn.grade}/100
                        </p>
                        <p className="text-[11px] text-zinc-400">{asn.feedback || "Great work!"}</p>
                     </div>
                  ) : (
                    <div className="border border-zinc-800 rounded-xl p-4 bg-blue-900/10">
                      <p className="text-[10px] font-bold text-white mb-1 flex items-center gap-2">Instructor Feedback (Pending)</p>
                      <p className="text-[10px] text-zinc-500 ml-1">Awaiting review from the instruction team...</p>
                    </div>
                  )}
                </div>
              )}
            </section>
          ))
        ) : (
          <div className="col-span-full py-20 text-center border border-dashed border-slate-700 rounded-2xl">
            <p className="text-zinc-500 italic text-sm">No {activeCategory.toLowerCase()} assignments found.</p>
          </div>
        )}
      </div>
    </main>
  );
}