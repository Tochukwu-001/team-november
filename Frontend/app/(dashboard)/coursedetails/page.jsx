import Image from "next/image";
import { ThemeColors } from "@/components/ThemeColors";
import Sidebar from "@/components/Sidebar";

/* ── Course module data ── */
const modules = [
  {
    id: 1,
    title: "Introduction to UX Design",
    duration: "35 mins",
    status: "completed", // completed | in-progress | locked
  },
  {
    id: 2,
    title: "User Research Methods",
    duration: "45 mins",
    status: "completed",
  },
  {
    id: 3,
    title: "Wireframe & Prototyping",
    duration: "50 mins",
    status: "in-progress",
  },
  {
    id: 4,
    title: "Design Systems",
    duration: "40 mins",
    status: "locked",
  },
  {
    id: 5,
    title: "Usability Testing",
    duration: "55 mins",
    status: "locked",
  },
];

/* ── Course includes data ── */
const courseIncludes = [
  {
    icon: "/assets/course-details-page/video-icon.svg",
    text: "12 video lessons",
  },
  {
    icon: "/assets/course-details-page/book-icon.svg",
    text: "8 downloadable resources",
  },
  {
    icon: "/assets/course-details-page/assignment-icon.svg",
    text: "3 assignments",
  },
  {
    icon: "/assets/course-details-page/discussion-icon.svg",
    text: "Discussion forum access",
  },
  {
    icon: "/assets/course-details-page/trophy-icon.svg",
    text: "Certificate of completion",
  },
];

export default function CourseDetails({ user = "Emeka Obi" }) {
  const progress = 72;
  const completedLessons = 8;
  const totalLessons = 12;

  return (
    <main className="flex min-h-screen">
      <Sidebar activeTab="Course Catalog" />

      {/* Main content area */}
      <section
        className="flex-1 flex flex-col min-w-0 bg-[#0D1321]"
        style={{
          fontFamily: "var(--font-inter), 'Inter', sans-serif",
        }}
      >
        {/* ===== Course Detail Header ===== */}
        <header
          id="course-detail-header"
          className="
            flex flex-row justify-between items-center
            px-[18px] py-[24px]
            bg-[#101723]
            border-b-[0.75px] border-solid border-[#7D7F82]

            max-[1024px]:flex-col max-[1024px]:gap-[16px] max-[1024px]:items-start
            max-[1024px]:px-[16px] max-[1024px]:py-[20px]

            max-[768px]:px-[14px] max-[768px]:py-[16px] max-[768px]:gap-[14px]
          "
        >
          {/* Left group  */}
          <div
            id="course-header-left"
            className="
              flex flex-row items-center gap-[11px]
              max-[768px]:gap-[8px] max-[768px]:flex-wrap
            "
          >
            <button
              id="course-view-btn"
              className="
                flex flex-row justify-center items-center
                px-[10px] py-[10px] gap-[10px]
                w-[60px] h-[39px]
                bg-[#4B4C4E] rounded-[8px]
                border-none cursor-pointer
                transition-colors duration-200
                hover:bg-[#5c5d5f]
                max-[768px]:w-[50px] max-[768px]:h-[34px] max-[768px]:px-[8px]
              "
            >
              <span className="text-[16px] font-normal leading-[125%] text-[#CEE0FD] whitespace-nowrap max-[768px]:text-[14px]">
                Back
              </span>
            </button>

            <h1
              id="course-title-header"
              className="
                text-[28px] font-bold leading-[125%] text-[#FAFCFF] m-0
                whitespace-nowrap
                max-[1024px]:text-[24px]
                max-[768px]:text-[20px]
              "
            >
              UI/UX Fundamentals
            </h1>
          </div>

          {/* Right group */}
          <div
            id="course-header-right"
            className="
              flex flex-row justify-between items-center gap-[25px]
              max-[1024px]:w-full max-[1024px]:justify-end
              max-[768px]:gap-[12px] max-[768px]:flex-wrap max-[768px]:justify-start
            "
          >
            <button
              id="course-progress-btn"
              className="
                flex flex-row justify-center items-center
                px-[16px] py-[16px] gap-[8px]
                h-[24px]
                bg-[#3E5C8E] rounded-[16px]
                border-none cursor-pointer
                transition-colors duration-200 hover:bg-[#4a6fa5]
                max-[768px]:px-[12px] max-[768px]:py-[12px] max-[768px]:h-[20px]
              "
            >
              <span className="text-[12px] font-bold leading-[125%] text-[#FAFCFF] whitespace-nowrap max-[768px]:text-[11px]">
                {progress}% Complete
              </span>
            </button>

            <button
              id="course-resume-btn"
              className="
                flex flex-row justify-center items-center
                px-[10px] py-[10px] gap-[10px]
                h-[39px]
                bg-[#0950C3] rounded-[8px]
                border-none cursor-pointer
                transition-colors duration-200 hover:bg-[#0a5fd6]
                max-[768px]:h-[34px] max-[768px]:px-[8px] max-[768px]:gap-[6px]
              "
            >
              <span className="text-[23px] font-bold leading-[125%] text-[#FAFCFF] whitespace-nowrap max-[1024px]:text-[20px] max-[768px]:text-[16px]">
                Resume course
              </span>
              <Image
                src="/assets/course-details-page/right-arrow.svg"
                alt="Arrow right"
                width={18}
                height={14}
              />
            </button>
          </div>
        </header>

        {/* ===== Page Body ===== */}
        <div
          id="course-detail-body"
          className="
            flex flex-row items-start
            gap-[41px] p-[22px]
            max-[1024px]:flex-col max-[1024px]:gap-[32px]
            max-[768px]:p-[14px] max-[768px]:gap-[24px]
          "
        >
          {/* ── Left Column  ── */}
          <div
            id="course-left-col"
            className="
              flex flex-col items-start gap-[40px]
              flex-1 min-w-0
              max-[1024px]:w-full
              max-[768px]:gap-[28px]
            "
          >
            {/* ── Course Overview Section ── */}
            <div
              id="course-overview"
              className="flex flex-col items-start gap-[27px] w-full max-[768px]:gap-[20px]"
            >
              {/* Course Thumbnail Banner */}
              <div
                id="course-thumbnail"
                className="
                  w-full h-[218px] bg-[#ADC7EB]
                  border border-solid border-[#4B4C4E]
                  rounded-[16px]
                  flex items-center justify-center
                  max-[768px]:h-[160px]
                "
              >
                <Image
                  src="/assets/course-details-page/palette-icon.svg"
                  alt="Course thumbnail"
                  width={60}
                  height={60}
                />
              </div>

              {/* Course Info Section  */}
              <div
                id="course-info"
                className="flex flex-col items-start gap-[12px] w-full"
              >
                {/* Title + metadata row */}
                <div className="flex flex-col items-start gap-[22px] w-full">
                  {/* Title block */}
                  <div className="flex flex-col items-start gap-[10px]">
                    <h2
                      id="course-title"
                      className="
                        text-[19px] font-bold leading-[125%] text-[#FAFCFF] m-0
                        max-[768px]:text-[17px]
                      "
                    >
                      UI/UX Fundamentals
                    </h2>

                    {/* Metadata row */}
                    <div
                      id="course-meta"
                      className="
                        flex flex-row items-center gap-[10px]
                        flex-wrap
                        max-[768px]:gap-[8px]
                      "
                    >
                      {/* Instructor */}
                      <div className="flex flex-row items-center gap-[8px]">
                        <Image
                          src="/assets/course-details-page/user-icon.svg"
                          alt="Instructor"
                          width={16}
                          height={16}
                        />
                        <span className="text-[12px] font-normal leading-[125%] text-[#ADC7EB] whitespace-nowrap">
                          {user}
                        </span>
                      </div>

                      {/* Lessons */}
                      <div className="flex flex-row items-center gap-[8px]">
                        <Image
                          src="/assets/course-details-page/lessons-icon.svg"
                          alt="Lessons"
                          width={16}
                          height={16}
                        />
                        <span className="text-[12px] font-normal leading-[125%] text-[#ADC7EB] whitespace-nowrap">
                          12 Lessons
                        </span>
                      </div>

                      {/* Duration */}
                      <div className="flex flex-row items-center gap-[8px]">
                        <Image
                          src="/assets/course-details-page/clock-icon.svg"
                          alt="Duration"
                          width={16}
                          height={16}
                        />
                        <span className="text-[12px] font-normal leading-[125%] text-[#ADC7EB] whitespace-nowrap">
                          6 hours
                        </span>
                      </div>

                      {/* Rating */}
                      <div className="flex flex-row items-center gap-[8px]">
                        <Image
                          src="/assets/course-details-page/star-icon.svg"
                          alt="Rating"
                          width={16}
                          height={16}
                        />
                        <span className="text-[12px] font-normal leading-[125%] text-[#ADC7EB] whitespace-nowrap">
                          4.9 (48 reviews)
                        </span>
                      </div>
                    </div>
                  </div>

                  {/* Category pill + description */}
                  <div className="flex flex-col items-start gap-[12px] w-full">
                    {/* Category pill button */}
                    <button
                      id="course-category-btn"
                      className="
                        flex flex-row justify-center items-center
                        px-[16px] py-[16px] gap-[8px]
                        h-[24px]
                        bg-[#0950C3] rounded-[16px]
                        border-none cursor-pointer
                        transition-colors duration-200 hover:bg-[#0a5fd6]
                      "
                    >
                      <span className="text-[12px] font-bold leading-[125%] text-[#FAFCFF] whitespace-nowrap">
                        UI/UX Design
                      </span>
                    </button>

                    {/* Course description */}
                    <p
                      id="course-description"
                      className="
                        text-[12px] font-normal leading-[125%] text-[#FAFCFF] m-0
                        max-w-[591px]
                        max-[768px]:max-w-full
                      "
                    >
                      Learn the core principles of user interface and experience
                      design. From research to wireframing, prototyping to
                      handoff — this course covers everything you need to work
                      as a professional UX designer at TrueMinds.
                    </p>
                  </div>
                </div>

                {/* Progress Bar Section */}
                <div
                  id="course-progress-bar"
                  className="flex flex-col items-start gap-[2px] w-full"
                >
                  {/* Progress bar track */}
                  <div className="relative w-full h-[13px] rounded-[12px] overflow-hidden">
                    {/* Background track */}
                    <div
                      className="absolute inset-0 rounded-[12px]"
                      style={{ background: "rgba(125, 127, 130, 0.5)" }}
                    />
                    {/* Filled portion */}
                    <div
                      className="absolute top-0 left-0 h-full rounded-l-[12px]"
                      style={{
                        width: `${progress}%`,
                        background:
                          "linear-gradient(270deg, #ADC7EB 0%, #627185 100%)",
                      }}
                    />
                  </div>

                  {/* Labels row */}
                  <div className="flex flex-row justify-between items-center w-full">
                    <span className="text-[12px] font-normal leading-[125%] text-[#0950C3]">
                      {completedLessons} of {totalLessons} lessons completed
                    </span>
                    <span className="text-[12px] font-bold leading-[125%] text-[#0950C3]">
                      {progress}%
                    </span>
                  </div>
                </div>
              </div>
            </div>

            {/* ── Course Modules Section ── */}
            <div
              id="course-modules"
              className="flex flex-col items-start gap-[32px] w-full max-[768px]:gap-[24px]"
            >
              <h2 className="text-[16px] font-bold leading-[125%] text-white m-0 w-full">
                Course Modules
              </h2>

              {/* Module cards container */}
              <div className="flex flex-col items-start gap-[20px] w-full max-[768px]:gap-[14px]">
                {modules.map((mod) => (
                  <ModuleCard key={mod.id} module={mod} />
                ))}
              </div>
            </div>
          </div>

          {/* ── Right Sidebar Column  ── */}
          <div
            id="course-right-col"
            className="
              flex flex-col items-start gap-[31px]
              w-[341px] shrink-0
              max-[1024px]:w-full
              max-[768px]:gap-[20px]
            "
          >
            {/* ── Your Instructor Card  ── */}
            <div
              id="instructor-card"
              className="
                flex flex-col items-start
                p-[24px_16px] gap-[20px]
                w-full
                bg-[#101723]
                border-[0.75px] border-solid border-[#7D7F82]
                rounded-[16px]
                max-[768px]:p-[16px_12px]
              "
            >
              <div className="flex flex-col items-start gap-[20px] w-full">
                <h3 className="text-[16px] font-bold leading-[125%] text-white m-0 w-full">
                  Your Instructor
                </h3>

                <div className="flex flex-col items-start gap-[10px]">
                  {/* Instructor avatar + name */}
                  <div className="flex flex-row items-end gap-[10px]">
                    {/* Avatar circle */}
                    <div className="w-[36px] h-[36px] bg-[#0950C3] rounded-full flex items-center justify-center shrink-0 relative">
                      <span className="text-[16px] font-bold leading-[125%] text-[#FAFCFF]">
                        EO
                      </span>
                    </div>

                    {/* Name + role */}
                    <div className="flex flex-col items-start gap-[2px]">
                      <span className="text-[16px] font-bold leading-[125%] text-[#FAFCFF]">
                        Emeka obi
                      </span>
                      <span className="text-[12px] font-normal leading-[125%] text-[#FAFCFF]">
                        Senior UX Designer
                      </span>
                    </div>
                  </div>

                  {/* Instructor bio */}
                  <p className="text-[16px] font-normal leading-[125%] text-[#DCE3EF] m-0 max-[768px]:text-[14px]">
                    8 years of UX experience across fintech, edtech, and
                    e-commerce. Currently leading design at TrueMinds
                    Innovation.
                  </p>
                </div>
              </div>
            </div>

            {/* ── Course Includes Card  ── */}
            <div
              id="course-includes-card"
              className="
                flex flex-col items-start
                p-[24px_16px] gap-[20px]
                w-full
                bg-[#101723]
                border-[0.75px] border-solid border-[#7D7F82]
                rounded-[16px]
                max-[768px]:p-[16px_12px]
              "
            >
              <div className="flex flex-col items-start gap-[20px]">
                <h3 className="text-[16px] font-bold leading-[125%] text-[#FAFCFF] m-0">
                  Course Includes
                </h3>

                {/* Items list */}
                <div className="flex flex-col items-start gap-[6px]">
                  {courseIncludes.map((item, idx) => (
                    <div
                      key={idx}
                      className="flex flex-row items-start gap-[6px]"
                    >
                      <Image
                        src={item.icon}
                        alt=""
                        width={16}
                        height={16}
                        className="shrink-0 mt-[2px]"
                      />
                      <span className="text-[16px] font-normal leading-[125%] text-[#FAFCFF] max-[768px]:text-[14px]">
                        {item.text}
                      </span>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}

/* ── Module Card Component ── */
function ModuleCard({ module }) {
  const { id, title, duration, status } = module;

  /* Determine which icon / indicator to show on the left square */
  const renderLeftIndicator = () => {
    if (status === "completed" || status === "in-progress") {
      const iconSrc =
        status === "completed"
          ? "/assets/course-details-page/check-icon.svg"
          : "/assets/course-details-page/play-icon.svg";
      return (
        <div className="w-[32px] h-[32px] bg-[#2F456A] rounded-[4px] flex items-center justify-center shrink-0 relative">
          <Image src={iconSrc} alt={status} width={24} height={24} />
        </div>
      );
    }
    /* locked — show module number */
    return (
      <div className="w-[32px] h-[32px] bg-[#4B4C4E] rounded-[4px] flex items-center justify-center shrink-0">
        <span className="text-[16px] font-bold leading-[125%] text-[#7D7F82]">
          {id}
        </span>
      </div>
    );
  };

  /* Determine right-side icon / button */
  const renderRightAction = () => {
    if (status === "completed") {
      return (
        <Image
          src="/assets/course-details-page/check-icon.svg"
          alt="Completed"
          width={24}
          height={24}
          className="shrink-0"
        />
      );
    }
    if (status === "in-progress") {
      return (
        <button
          id={`module-resume-${id}`}
          className="
            flex flex-row justify-center items-center
            px-[16px] py-[16px] gap-[8px]
            h-[25px]
            bg-[#0950C3] rounded-[16px]
            border-none cursor-pointer
            transition-colors duration-200 hover:bg-[#0a5fd6]
          "
        >
          <span className="text-[12px] font-bold leading-[125%] text-[#FAFCFF] whitespace-nowrap">
            Resume
          </span>
        </button>
      );
    }
    /* locked */
    return (
      <Image
        src="/assets/course-details-page/lock-icon.svg"
        alt="Locked"
        width={24}
        height={24}
        className="shrink-0"
      />
    );
  };

  return (
    <div
      id={`module-card-${id}`}
      className="
        flex flex-row justify-between items-center
        px-[10px] py-[20px]
        w-full
        bg-[#101723]
        border border-solid border-[#0950C3]
        rounded-[16px]
        max-[768px]:px-[8px] max-[768px]:py-[14px]
      "
    >
      {/* Left: indicator + title/duration (Frame 169) */}
      <div className="flex flex-row items-center gap-[9px]">
        {renderLeftIndicator()}

        {/* Title + duration (Frame 168) */}
        <div className="flex flex-col items-start gap-[2px]">
          <span className="text-[12px] font-bold leading-[125%] text-[#FAFCFF]">
            {title}
          </span>
          <span className="text-[12px] font-normal leading-[125%] text-[#7D7F82]">
            {duration}
          </span>
        </div>
      </div>

      {/* Right: action indicator */}
      {renderRightAction()}
    </div>
  );
}
