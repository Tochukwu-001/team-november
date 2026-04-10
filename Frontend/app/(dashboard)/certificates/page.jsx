"use client";
import { useEffect, useMemo, useState } from "react";
import Image from "next/image";
import { ThemeColors } from "@/components/ThemeColors";
import client from "@/lib/client";

export default function CertificatesPage() {
  const [certificates, setCertificates] = useState([]);
  const [userName, setUserName] = useState(""); // State for the user's name from profile
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState("");

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        
        // 1. Fetch User Profile for the name fallback
        const profileRes = await client.get("/Profile/me");
        if (profileRes.data?.data?.fullName) {
          setUserName(profileRes.data.data.fullName);
        }

        // 2. Fetch Certificates
        const certRes = await client.get("/Certificate/me");
        const items = certRes.data?.data;

        if (Array.isArray(items)) {
          setCertificates(items);
        }
      } catch (error) {
        console.error("Fetch error:", error);
        setMessage("Unable to load certificates right now.");
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, []);

  const activeCertificate = useMemo(() => {
    return certificates.length > 0 ? certificates[0] : null;
  }, [certificates]);

  // Determine the display name: Priority to Certificate data, fallback to Profile name
  const displayName = activeCertificate?.userFullName || userName || 'Student Name';

  const completionValue = useMemo(() => {
    if (!activeCertificate) return 72; // Default preview value
    return Math.max(0, Math.min(100, Math.round(Number(activeCertificate.finalScore || 0))));
  }, [activeCertificate]);

  const completionRemaining = 100 - completionValue;

  return (
    <main
      className="min-h-screen font-sans text-white"
      style={{ backgroundColor: ThemeColors.bgBlue }}
    >
      <div className="flex-1 w-full flex flex-col min-h-screen">
        {/* Top Bar */}
        <div
          className="flex items-center justify-between px-8 py-5"
          style={{
            backgroundColor: ThemeColors.bgBlue,
            borderBottom: "0.75px solid rgba(214,227,245,0.25)",
          }}
        >
          <span className="text-white font-bold text-3xl tracking-tight">
            My Certificates
          </span>
          <button
            className="flex items-center gap-2 px-5 py-3 rounded-2xl font-bold text-lg opacity-50 hover:opacity-100 transition-opacity"
            style={{
              border: "0.5px solid rgba(214,227,245,0.4)",
              background: "transparent",
              color: "#0950C3",
            }}
          >
            Share All
          </button>
        </div>

        {/* Content */}
        <div className="flex flex-col gap-5 p-7 flex-1">
          {message && (
            <div className="rounded-lg border border-[#4B4C4E] bg-[#0D1522] px-4 py-3 text-sm text-[#CEE0FD]">
              {message}
            </div>
          )}

          {/* Progress Banner */}
          <div
            className="flex flex-wrap items-center gap-6 px-6 py-7 rounded-2xl"
            style={{ backgroundColor: ThemeColors.bgBlue, border: "1px solid #0950C3" }}
          >
            <Image src="/assets/certificates/certficates-page-icon.svg" width={32} height={32} alt="icon" />
            <div className="flex-1 min-w-48">
              <h2 className="text-white font-bold text-xl mb-1">
                You are {completionRemaining}% away from your next certificate!
              </h2>
              <p className="text-sm opacity-70">
                {activeCertificate
                  ? `Latest earned: ${activeCertificate.courseTitle}. Keep progressing to unlock more.`
                  : "Complete your current course to earn your first certificate."}
              </p>
              <div className="w-full h-2.5 rounded-full bg-gray-500/40 mt-3 max-w-[220px]">
                <div
                  className="h-full rounded-full transition-all duration-500"
                  style={{
                    width: `${completionValue}%`,
                    background: "linear-gradient(90deg, #627185 0%, #ADC7EB 100%)",
                  }}
                />
              </div>
            </div>
            <button className="bg-[#0950C3] text-white px-4 py-2.5 rounded-lg font-bold text-sm flex items-center gap-2">
              Continue <Image src="/assets/certificates/arrow-right.svg" width={18} height={14} alt="arrow" />
            </button>
          </div>

          {/* Certificate Card */}
          <div
            className="relative flex-1 rounded-2xl overflow-hidden flex flex-col justify-between p-8 md:p-14 lg:p-20"
            style={{
              background: `linear-gradient(180deg, ${ThemeColors.bgBlue} 0%, #2D3D58 48%, #051838 80%)`,
              border: "1px solid #0950C3",
              minHeight: "560px",
            }}
          >
            {/* Stars UI omitted for brevity - keep your existing Star divs here */}

            <div className="flex flex-col items-center gap-2.5 z-10 w-full max-w-2xl mx-auto text-center mt-6">
              <span className="font-bold text-xs tracking-widest uppercase text-[#DCE3EF]">
                Certificate of Completion
              </span>
              <p className="font-bold text-2xl md:text-3xl text-[#DCE3EF] mt-4">
                TrueMinds Innovation
              </p>
              <span className="text-sm opacity-70">This is to certify that</span>

              {/* DYNAMIC NAME DISPLAY */}
              <p className="font-bold text-3xl md:text-5xl text-[#0950C3] mt-2 md:mt-4 break-words">
                {loading ? "Loading..." : displayName}
              </p>

              <span className="text-sm md:text-base font-medium opacity-90 mt-4">
                has successfully completed {activeCertificate?.courseTitle || 'Your Current Course'}
              </span>
              <span className="text-sm md:text-base font-medium opacity-90">
                with a final score of {activeCertificate?.finalScore ?? 0}/100
              </span>
              
              <div className="flex w-full items-center gap-2.5 my-6">
                <div className="flex-1 h-px bg-[#0950C3]" />
                <Image src="/assets/certificates/blue-star.svg" width={15} height={15} alt="star" />
                <div className="flex-1 h-px bg-[#0950C3]" />
              </div>
            </div>

            {/* Footer */}
            <div className="flex flex-col sm:flex-row justify-between items-center sm:items-end w-full z-10 gap-8 mt-auto px-4">
              <div className="text-center sm:text-left">
                <p className="font-bold text-xl text-white">Chioma Adeyemi</p>
                <div className="my-2 h-px w-48 bg-white/20" />
                <p className="text-xs text-[#D6E3F5]">Instructor — TrueMinds Innovation</p>
              </div>

              <div className="text-center sm:text-right text-[#ADC7EB] text-sm">
                <p>Issued: {activeCertificate?.issuedAt ? new Date(activeCertificate.issuedAt).toLocaleDateString() : '---'}</p>
                <p>ID: {activeCertificate?.certificateNumber || 'TMI-PENDING'}</p>
                <p className="text-[#0950C3] font-medium mt-1">Verify at trueminds.ng/cert</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  );
}