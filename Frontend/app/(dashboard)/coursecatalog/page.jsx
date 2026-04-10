"use client";
import { useState, useEffect } from "react";
import CourseCard from "@/components/CourseCard";
import { DarkSunIcon, LightningIcon, PaletteIcon } from "@/components/Icons";
import Link from "next/link";
import client from "@/lib/client";

export default function CourseCatalog() {
  const [userDiscipline, setUserDiscipline] = useState("");
  const [activeTab, setActiveTab] = useState("All courses");
  const [loading, setLoading] = useState(true);

  // 1. Fetch User Profile to get the Discipline
  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const response = await client.get("/Profile/me");
        const discipline = response.data?.data?.personalInformation?.discipline || "";
        setUserDiscipline(discipline);
        
        // Auto-set the active tab based on discipline if possible
        if (discipline.toLowerCase().includes("design")) setActiveTab("Design");
        else if (discipline.toLowerCase().includes("engineer")) setActiveTab("Engineering");
      } catch (err) {
        console.error("Error fetching profile for catalog:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchProfile();
  }, []);

  const courses = [
    {
      title: "UI/UX Fundamentals",
      category: "Design", // Normalized category for filtering
      displayCategory: "UI/UX DESIGN",
      description: "Learn the core principles of user interface design and experience from scratch",
      lessons: 12,
      duration: "-6h",
      author: "Emeka Obi",
      initials: "EO",
      bgColor: "bg-[#9db1d6]",
      status: "Enrolled",
      icon: <PaletteIcon className="w-12.5 h-12.5" />,
      progress: 55,
    },
    {
      title: "Product Thinking",
      category: "Product",
      displayCategory: "PRODUCT MANAGEMENT",
      description: "Understand how to think like a product manager and solve user problems effectively",
      lessons: 8,
      duration: "-6h",
      author: "Segun Obe",
      initials: "SO",
      bgColor: "bg-[#3a4468]",
      status: "Enrolled",
      icon: <DarkSunIcon className="w-12.5 h-12.5" />,
      progress: 30,
    },
    {
      title: "Agile & Scrum Mastery",
      category: "Engineering",
      displayCategory: "ENGINEERING",
      description: "Master modern agile methodologies and learn to ship products fast and collaboratively",
      lessons: 10,
      duration: "-5h",
      author: "Tunde Ige",
      initials: "TI",
      bgColor: "bg-[#e6a8ff]",
      status: "New",
      icon: <LightningIcon className="w-12.5 h-12.5" />,
    },
    {
      title: "Data-Driven Decision Making",
      category: "Data Science",
      displayCategory: "DATA SCIENCE",
      description: "Use data to drive product decisions, identify trends and validate hypothesis",
      lessons: 15,
      duration: "-5h",
      author: "Ife Babs",
      initials: "IB",
      bgColor: "bg-[#1a6b46]",
      icon: "📊",
    },
    {
      title: "Communications & Presentation",
      category: "Business",
      displayCategory: "SOFT SKILLS",
      description: "Build confidence, clarity, and storytelling skills for professional settings",
      lessons: 6,
      duration: "-5h",
      author: "Herny Jaji",
      initials: "HJ",
      bgColor: "bg-[#2a4e21]",
      icon: "🎤",
    },
    {
      title: "Building APIs With Node.js",
      category: "Engineering",
      displayCategory: "BACKEND DEVELOPMENT",
      description: "Develop powerful systems and APIs that run behind the scenes",
      lessons: 10,
      duration: "-5h",
      author: "Adeola Kore",
      initials: "AK",
      bgColor: "bg-[#fb7c56]",
      icon: "🧩",
    },
    {
      title: "Design Principles",
      category: "Design",
      displayCategory: "GRAPHICS DESIGN",
      description: "Learn to create stunning visuals and user friendly designs",
      lessons: 10,
      duration: "-9h",
      author: "Samuel Moon",
      initials: "SM",
      bgColor: "bg-[#f5e642]",
      icon: "✨",
    },
    {
      title: "HTML & CSS Fundamentals",
      category: "Engineering",
      displayCategory: "FRONTEND DEVELOPMENT",
      description: "Build responsive and interactive user interfaces for web",
      lessons: 11,
      duration: "-8h",
      author: "Aaron Akor",
      initials: "AA",
      bgColor: "bg-[#2ec4b6]",
      icon: "🖥️",
    },
    {
      title: "Git & Version Control",
      category: "Engineering",
      displayCategory: "ENGINEERING",
      description: "Learn how to collaborate on code using Git, Github, and branching strategies",
      lessons: 13,
      duration: "-5h",
      author: "Olumide Obe",
      initials: "OO",
      bgColor: "bg-[#f4a5b8]",
      icon: "🔒",
    },
  ];

  const tabs = [
    "All courses",
    "Design",
    "Engineering",
    "Product",
    "Data Science",
    "Business",
  ];

  // 2. Filter Logic: If "All courses" is selected, we show all. Otherwise, we match the tab name.
  const filteredCourses = activeTab === "All courses" 
    ? courses 
    : courses.filter(course => course.category === activeTab);

  return (
    <main className="min-h-screen bg-[#0f172a] p-8">
      {/* Page Header */}
      <div className="mb-2 flex items-center justify-between">
        <h1 className="text-xl font-bold text-white">Course Catalog</h1>
        {!loading && userDiscipline && (
          <div className="text-[10px] bg-blue-500/20 text-blue-400 px-3 py-1 rounded-full border border-blue-500/30 uppercase font-bold tracking-widest">
            {userDiscipline} Track
          </div>
        )}
      </div>

      {/* Tab Bar */}
      <div className="flex items-center justify-between border-b border-white/10 mb-8 overflow-x-auto">
        <div className="flex gap-1 min-w-max">
          {tabs.map((tab) => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`px-4 py-3 text-sm font-medium transition-colors duration-150 relative
                ${activeTab === tab ? "text-white" : "text-slate-400 hover:text-white"}`}
            >
              {tab}
              {activeTab === tab && (
                <div className="absolute bottom-0 left-0 w-full h-0.5 bg-blue-400"></div>
              )}
            </button>
          ))}
        </div>

        {/* My Courses link */}
        <Link href="/dashboard" className="flex items-center gap-1.5 text-slate-300 text-sm font-medium hover:text-white transition-colors mb-2 whitespace-nowrap ml-4">
          <span className="text-blue-400 text-[10px]">▶</span>
          Go to Dashboard
        </Link>
      </div>

      {/* Course Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredCourses.length > 0 ? (
          filteredCourses.map((course, index) => (
            <Link key={index} href={"/lesson"}>
              {/* Note: I added 'category: course.displayCategory' inside the mapping to match your UI component expectations */}
              <CourseCard course={{...course, category: course.displayCategory}} />
            </Link>
          ))
        ) : (
          <div className="col-span-full py-20 text-center">
            <p className="text-slate-500 italic">No courses available in this category yet.</p>
          </div>
        )}
      </div>
    </main>
  );
}