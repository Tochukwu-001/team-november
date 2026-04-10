export default function CourseCard({ course }) {
  return (
    <div className="rounded-2xl overflow-hidden border-[0.76px] border-[#D6E3F5] flex flex-col transition-transform duration-200 hover:-translate-y-1 hover:shadow-xl hover:shadow-black/30">
      {/* 1. Top Color Banner */}
      <div
        className={`h-43 rounded-tl-2xl rounded-tr-2xl ${course.bgColor} relative flex items-center justify-center`}
      >
        {course.status && (
          <span
            className={`absolute top-4 left-4 w-25.25 font-bold text-[#FAFCFF] text-[16px] p-4 rounded-2xl leading-[125%] 
              ${course.status === "New" ? "bg-[#3E5C8E]" : "bg-[#3E5C8E]"}`}
          >
            {course.status}
          </span>
        )}
        <span className="text-5xl select-none">{course.icon}</span>
      </div>

      {/* 2. Text Content */}
      <div className="p-5 flex flex-col gap-1.5 grow">
        <span className="text-[#0950C3] text-[12px] font-bold uppercase tracking-widest">
          {course.category}
        </span>
        <h3 className="text-[#FAFCFF] text-[16px] font-bold leading-[125%] mt-0.5">
          {course.title}
        </h3>
        <p className="text-[#FAFCFF] text-[12px]  line-clamp-2 mt-1">
          {course.description}
        </p>

        {/* Progress bar — only shown for enrolled courses */}
        {course.progress !== undefined && (
          <div className="mt-3">
            <div className="h-3.25 bg-[#7D7F82] rounded-full overflow-hidden">
              <div
                className="h-full bg-[#ADC7EB] rounded-full transition-all duration-500"
                style={{ width: `${course.progress}%` }}
              />
            </div>
          </div>
        )}
      </div>

      {/* 3. Footer */}
      <div className="border-t-[#4B4C4E] w-[90%] mx-auto border-[0.76px] p-3 flex items-center justify-between mt-auto mb-3">
        <div className="flex items-center gap-2">
          <div className="w-5.75 h-5 rounded-full bg-blue-600 flex items-center justify-center text-white text-[10px] font-bold shrink-0">
            {course.initials}
          </div>
          <span className="text-[#7D7F82] text-[16px] font-normal leading-[125%]">
            {course.author}
          </span>
        </div>
        <span className="text-[#7D7F82] text-[16px] font-normal leading-[125%]">
          {course.lessons} lessons
          {course.duration ? ` ${course.duration}` : ""}
        </span>
      </div>
    </div>
  );
}
