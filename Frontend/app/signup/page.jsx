"use client";
import { useState } from "react";
import Image from "next/image";
import client from "@/lib/client";
import Link from "next/link";

export default function Signup() {

  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    discipline: "UI/UX Design",
    password: "",
    confirmPassword: "",
  });

  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");
  const [errors, setErrors] = useState({});

  const handleChange = (e) => {
    const { name, value } = e.target;

    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    const errorKey = name.charAt(0).toUpperCase() + name.slice(1);
    if (errors[errorKey]) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[errorKey];
        return newErrors;
      });
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");

    if (formData.password !== formData.confirmPassword) {
      setErrors({ ConfirmPassword: ["Passwords do not match!"] });
      setLoading(false);
      return;
    }

    try {
      console.log(formData);      
      const response = await client.post('/Auth/register', formData);
      console.log(response);      
      setMessage("Registration Successful! Redirecting in 2 seconds...");
      setErrors({});

      setFormData({
        firstName: "",
        lastName: "",
        email: "",
        discipline: "UI/UX Design",
        password: "",
        confirmPassword: "",
      });

      setTimeout(() => window.location.href = "/login", 2000);

    } catch (error) {
      if (error.response && error.response.status === 400) {
        setErrors(error.response.data.errors || {});
      } else {
        setMessage("Server error. Please try again later.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <section className="grid min-h-dvh grid-cols-1 items-stretch p-4 md:grid-cols-2 md:p-6">
      <div className="h-full border-b border-gray-500 pb-6 pt-8 md:border-b-0 md:border-r md:pb-0 md:pr-6">
        <div>
          <Image src={"/logo.svg"} alt="logo" width={500} height={500} className="w-20 h-20 mb-5" />
          <h1 className="text-4xl font-bold mb-3 text-white">
            Start Your
            <br /> <span className="text-[#0950C3]">Journey</span> Today
          </h1>
          <p className="text-[#7D7F82] mb-4">
            Join hundreds of interns building real world skills on TalentFlow
          </p>
        </div>
        <div className="w-full max-w-[520px]">
          <div className="flex flex-row mb-4 gap-2 items-center">
            <div className="p-2 bg-[#314568] rounded-xl">
              <Image src="/Group.png" alt="Logo" width={20} height={20} />
            </div>
            <p>
              <span className="text-white font-bold">Unique ID Assigned</span>
              <span className="text-[#7D7F82] mb-4">
                {" "}
                - every signup gets a permanent identifier
              </span>
            </p>
          </div>
          <div className="flex flex-row mb-4 gap-2 items-center">
            <div className="p-2 bg-[#314568] rounded-xl">
              <Image src="/fill.png" alt="Logo" width={100} height={100} className="w-5 h-5" />
            </div>
            <p>
              <span className="text-white font-bold">Team Placement</span>
              <span className="text-[#7D7F82] mb-4">
                {" "}
                - Auto-assigned to your <br />
                discipline team
              </span>
            </p>
          </div>
          <div className="flex flex-row mb-4 gap-2 items-center">
            <div className="p-2 bg-[#314568] rounded-xl">
              <Image src="/target.png" alt="Logo" width={20} height={20} />
            </div>
            <p>
              <span className="text-white font-bold">Role-Based Access</span>
              <span className="text-[#7D7F82] mb-4">
                {" "}
                - Personalized for Leaner, Mentor, <br />
                or Admin
              </span>
            </p>
          </div>
        </div>
      </div>
      <div className="flex items-start justify-center p-4 md:min-h-screen md:p-6">
        <form onSubmit={handleSubmit} className="w-full max-w-sm">
          <h1 className="text-white text-3xl font-bold mb-1">Create Account</h1>

          <div className="flex items-center gap-2">
            <p className="text-stone-400 text-sm">Sign In</p>
            <Link href={"/login"} className="text-base font-semibold text-blue-600">Log In</Link>
          </div>

          {/* Status Message for non-field specific errors (like server 500) */}
          {message && <p className="text-sm my-2 text-blue-400">{message}</p>}

          {/* ── First Name + Last Name ── */}
          <div className="flex gap-3 mb-4 mt-6">
            <div className="flex-1">
              <label className="text-white text-sm font-semibold block mb-2">First Name</label>
              <input
                type="text"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                placeholder="Adaeze"
                className={`w-full border ${errors.FirstName ? 'border-red-500' : 'border-gray-300'} bg-transparent text-white text-sm rounded-lg px-3 py-2.5 outline-none`}
                required
              />
              {errors.FirstName && (
                <p className="text-red-500 text-xs mt-1">{errors.FirstName[0]}</p>
              )}
            </div>
            <div className="flex-1">
              <label className="text-white text-sm font-semibold block mb-2">Last Name</label>
              <input
                type="text"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                placeholder="Okoro"
                className={`w-full border ${errors.LastName ? 'border-red-500' : 'border-gray-300'} bg-transparent text-white text-sm rounded-lg px-3 py-2.5 outline-none`}
                required
              />
              {errors.LastName && (
                <p className="text-red-500 text-xs mt-1">{errors.LastName[0]}</p>
              )}
            </div>
          </div>

          {/* ── Email Address ── */}
          <div className="mb-4">
            <label className="text-white text-sm font-semibold block mb-2">Email Address</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              placeholder="you@trueminds.ng"
              className={`w-full border ${errors.Email ? 'border-red-500' : 'border-gray-300'} bg-transparent text-white text-sm rounded-lg px-3 py-2.5 outline-none`}
              required
            />
            {errors.Email && (
              <p className="text-red-500 text-xs mt-1">{errors.Email[0]}</p>
            )}
          </div>

          {/* ── Discipline ── */}
          <div className="mb-4">
            <label className="text-white text-sm font-semibold block mb-2">Discipline</label>
            <div className="relative">
              <select
                name="discipline"
                value={formData.discipline}
                onChange={handleChange}
                className={`w-full border ${errors.Discipline ? 'border-red-500' : 'border-gray-300'} bg-transparent text-white text-sm rounded-lg px-3 py-2.5 outline-none appearance-none cursor-pointer`}
              >
                <option value="UI/UX Design" className="text-black">UI/UX Design</option>
                <option value="Front-end Engineering" className="text-black">Front-end Engineering</option>
                <option value="Back-end Engineering" className="text-black">Back-end Engineering</option>
                <option value="Mobile Development" className="text-black">Mobile Development</option>
                <option value="Data Analysis" className="text-black">Data Analysis</option>
                <option value="Digital Marketing" className="text-black">Digital Marketing</option>
                <option value="Content Strategy" className="text-black">Content Strategy</option>
                <option value="DevOps" className="text-black">DevOps</option>
                <option value="QA Engineering" className="text-black">QA Engineering</option>
                <option value="Design" className="text-black">Design</option>
              </select>
              <div className="pointer-events-none absolute inset-y-0 right-3 flex items-center">
                <svg className="w-4 h-4 text-[#8b949e]" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
                </svg>
              </div>
            </div>
            {errors.Discipline && (
              <p className="text-red-500 text-xs mt-1">{errors.Discipline[0]}</p>
            )}
          </div>

          {/* ── Password ── */}
          <div className="mb-4">
            <label className="text-white text-sm font-semibold block mb-2">Password</label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="Minimum 8 characters"
              className={`w-full border ${errors.Password ? 'border-red-500' : 'border-gray-300'} bg-transparent text-white text-sm rounded-lg px-3 py-2.5 outline-none`}
              required
            />
            {errors.Password && (
              <p className="text-red-500 text-xs mt-1">{errors.Password[0]}</p>
            )}
          </div>

          {/* ── Confirm Password ── */}
          <div className="mb-6">
            <label className="text-white text-sm font-semibold block mb-2">Confirm Password</label>
            <input
              type="password"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              placeholder="Re-enter password"
              className={`w-full border ${errors.ConfirmPassword ? 'border-red-500' : 'border-gray-300'} bg-transparent text-white text-sm rounded-lg px-3 py-2.5 outline-none`}
              required
            />
            {errors.ConfirmPassword && (
              <p className="text-red-500 text-xs mt-1">{errors.ConfirmPassword[0]}</p>
            )}
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white text-sm font-semibold py-3 rounded-lg transition-colors"
          >
            {loading ? "Registering..." : "Get Started"}
          </button>
        </form>
      </div>
    </section>
  );
}
