import { ThemeColors } from '@/components/ThemeColors';

export default function TeamAllocation() {
  const teams = [
    {
      name: 'Design x Engineering',
      icon: '🎨',
      count: '8 Members',
      members: [
        { name: 'Adaeze Okoro', role: 'UI/UX Design', initial: 'AO', color: 'bg-blue-600', isUser: true },
        { name: 'Kolade Ige', role: 'Front-end Engineering', initial: 'KI', color: 'bg-blue-700' },
        { name: 'Fatimat Aliu', role: 'Project Management', initial: 'FA', color: 'bg-blue-800' },
        { name: 'Tunde Okafor', role: 'Back-end Engineering', initial: 'TO', color: 'bg-red-700' },
      ]
    },
    {
      name: 'Data & Product',
      icon: '💡',
      count: '6 Members',
      members: [
        { name: 'Ife Badmus', role: 'Data Science', initial: 'IB', color: 'bg-red-700' },
        { name: 'Chioma Adeyemi', role: 'Product Management', initial: 'CA', color: 'bg-blue-700' },
        { name: 'Moses Okeke', role: 'Data Analysis', initial: 'MO', color: 'bg-slate-600' },
      ]
    },
    {
      name: 'Growth & Marketing',
      icon: '📈',
      count: '7 Members',
      members: [
        { name: 'Yemi Adewale', role: 'Digital Marketing', initial: 'YA', color: 'bg-slate-600' },
        { name: 'Ruth Nwosu', role: 'Content Strategy', initial: 'RN', color: 'bg-blue-800' },
      ]
    },
    {
      name: 'DevOps & QA',
      icon: '🔄',
      count: '5 Members',
      members: [
        { name: 'Emeka Agu', role: 'DevOps', initial: 'EA', color: 'bg-blue-800' },
        { name: 'Sade Dada', role: 'QA Engineering', initial: 'SD', color: 'bg-blue-600' },
      ]
    }
  ];

  return (
    <main style={{ backgroundColor: ThemeColors.bgBlue }} className="min-h-screen text-zinc-300 font-sans p-4 md:p-8">
      
      {/* Header Section */}
      <div className="flex items-center justify-between mb-8 border-b border-zinc-800 pb-6">
        <h1 className="text-2xl font-bold text-white">Team Allocation</h1>
        <div className="bg-blue-600/20 border border-blue-500/30 px-3 py-1 rounded-full">
          <p className="text-[10px] font-bold text-blue-400">Cohort 3. 52 Interns</p>
        </div>
      </div>

      {/* Teams Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        {teams.map((team, i) => (
          <div key={i} className="border border-slate-400 rounded-2xl p-5 bg-transparent min-h-100">
            
            {/* Team Header */}
            <div className="flex items-start gap-3 mb-6">
              <div className="w-10 h-10 bg-slate-800 border border-slate-700 rounded-lg flex items-center justify-center text-xl">
                {team.icon}
              </div>
              <div>
                <h2 className="text-lg font-bold text-white leading-tight">{team.name}</h2>
                <p className="text-xs text-blue-500 font-medium">{team.count}</p>
              </div>
            </div>

            {/* Member List */}
            <div className="space-y-5">
              {team.members.map((member, j) => (
                <div key={j} className="flex items-center gap-3">
                  <div className={`${member.color} w-9 h-9 rounded-full flex items-center justify-center text-white font-bold text-xs shrink-0`}>
                    {member.initial}
                  </div>
                  <div className="overflow-hidden">
                    <p className="text-sm font-bold text-white truncate">
                      {member.name} {member.isUser && <span className="text-blue-500 font-medium ml-1">(You)</span>}
                    </p>
                    <p className="text-[11px] text-zinc-500 truncate">{member.role}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </main>
  );
}