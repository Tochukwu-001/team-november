import { ThemeColors } from '@/components/ThemeColors';

export default function DiscussionForum() {
  const forumPosts = [
    {
      votes: '14',
      title: 'Best practices for designing accessible color palettes?',
      description: 'I’ve been working on the color system for our design and struggling with contrast ratios. Anyone have tips on tools or processes for WCAG compliance?',
      author: 'Adaeze Okoro',
      time: '2h ago',
      replies: '8',
      tags: [
        { label: 'UI/UX Design', bg: 'bg-rose-100/10', text: 'text-rose-200' },
        { label: 'Accessibility', bg: 'bg-blue-600', text: 'text-white' }
      ]
    },
    {
      votes: '22',
      title: 'How do you run user interviews remotely?',
      description: 'For the product thinking assignment, I need to conduct 3 user interviews. Any advice on sectioning remote session using zoom.',
      author: 'Kolade Ige',
      time: '5h ago',
      replies: '12',
      tags: [
        { label: 'Product', bg: 'bg-orange-200/20', text: 'text-orange-200' },
        { label: 'Research', bg: 'bg-red-600', text: 'text-white' }
      ]
    },
    {
      votes: '09',
      title: 'Week 6 cohort meet up - who\'s joining?',
      description: 'Organizing an informal virtual hangout for Cohort 3 on Friday at 6PM.',
      author: 'Fatima Aliyu',
      time: '1 day ago',
      replies: '24',
      tags: [
        { label: 'General', bg: 'bg-green-600', text: 'text-white' }
      ]
    }
  ];

  const contributors = [
    { name: 'Ife Badmus', posts: '48 Posts', initial: 'IB', color: 'bg-red-700', trend: '🔥' },
    { name: 'Chioma Adeyemi', posts: '31 Posts', initial: 'CA', color: 'bg-blue-700' },
    { name: 'Moses Okeke', posts: '28 Posts', initial: 'MO', color: 'bg-slate-600' },
  ];

  return (
    <main style={{ backgroundColor: ThemeColors.bgBlue }} className="min-h-screen text-zinc-300 font-sans p-4 md:p-8">
      
      {/* Top Header */}
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-2xl font-bold text-white">Discussion Forum</h1>
        <button className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg text-sm font-semibold flex items-center gap-2">
          <span className="text-lg">+</span> New Trends
        </button>
      </div>

      <div className="grid md:grid-cols-[1fr_320px] gap-8">
        
        {/* Main Content: Forum List */}
        <div className="space-y-6">
          <h2 className="text-xl font-bold text-white mb-4">All UI/UX Product General Q&A</h2>
          
          {forumPosts.map((post, i) => (
            <div key={i} className="border border-slate-400 rounded-2xl p-5 flex gap-5 bg-transparent">
              {/* Vote Controls */}
              <div className="flex flex-col items-center gap-1">
                <button className="w-8 h-8 flex items-center justify-center bg-slate-800 rounded-md border border-slate-700 text-zinc-400 hover:text-white">▲</button>
                <span className="text-lg font-bold text-white">{post.votes}</span>
                <button className="w-8 h-8 flex items-center justify-center bg-slate-800 rounded-md border border-slate-700 text-zinc-400 hover:text-white">▼</button>
              </div>

              {/* Post Content */}
              <div className="flex-1">
                <h3 className="text-lg font-bold text-white mb-2">{post.title}</h3>
                <p className="text-sm text-zinc-400 leading-relaxed mb-4">{post.description}</p>
                
                <div className="flex flex-wrap items-center gap-3">
                  {post.tags.map((tag, j) => (
                    <span key={j} className={`${tag.bg} ${tag.text} text-[11px] font-bold px-3 py-1 rounded-full`}>
                      {tag.label}
                    </span>
                  ))}
                  <div className="flex items-center gap-1.5 text-xs text-zinc-500 ml-2">
                    <span>{post.author}</span>
                    <span>{post.time}</span>
                    <span>•</span>
                    <span>{post.replies} replies</span>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>

        {/* Sidebar: Top Contributors */}
        <aside className="space-y-4">
          <div className="border border-slate-400 rounded-2xl p-6">
            <h3 className="text-lg font-bold text-white mb-6">Top Contributors</h3>
            <div className="space-y-5">
              {contributors.map((user, i) => (
                <div key={i} className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <div className={`${user.color} w-10 h-10 rounded-full flex items-center justify-center text-white font-bold text-sm`}>
                      {user.initial}
                    </div>
                    <div>
                      <p className="text-sm font-bold text-white leading-tight">{user.name}</p>
                      <p className="text-[11px] text-zinc-500">{user.posts}</p>
                    </div>
                  </div>
                  {user.trend && <span className="text-lg">{user.trend}</span>}
                </div>
              ))}
            </div>
          </div>
        </aside>

      </div>
    </main>
  );
}