import { useState } from "react";

interface LeadForm {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  jobTitle: string;
  title: string;
  source: string;
  estimatedValue: number | string;
}

interface FormErrors {
  firstName: string | null;
  lastName: string | null;
  email: string | null;
  phone: string | null;
  jobTitle: string | null;
  title: string | null;
  source: string | null;
  estimatedValue: string | null;
}

export const LeadPage = () => {
  const [step, setStep] = useState<number>(0);
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);

  const [lead, setLead] = useState<LeadForm>({
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    jobTitle: "",
    title: "",
    source: "",
    estimatedValue: 0.0,
  });

  const [errors, setErrors] = useState<FormErrors>({
    firstName: null,
    lastName: null,
    email: null,
    phone: null,
    jobTitle: null,
    title: null,
    source: null,
    estimatedValue: null,
  });

  const handleChange = (
    e: any
  ) => {
    const { id, value } = e.target;
    setLead((prev) => ({ ...prev, [id]: value }));

    // Clear error dynamically on edit
    if (errors[id as keyof FormErrors]) {
      setErrors((prev) => ({ ...prev, [id]: null }));
    }
  };

  const validateStep0 = (): boolean => {
    const newErrors: Partial<FormErrors> = {};
    if (!lead.firstName.trim()) newErrors.firstName = "First name is required.";
    if (!lead.lastName.trim()) newErrors.lastName = "Last name is required.";
    if (!lead.email.trim()) {
      newErrors.email = "Email is required.";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(lead.email)) {
      newErrors.email = "Invalid email format.";
    }

    setErrors((prev) => ({ ...prev, ...newErrors }));
    return !newErrors.firstName && !newErrors.lastName && !newErrors.email;
  };

  const validateStep1 = (): boolean => {
    const newErrors: Partial<FormErrors> = {};
    if (!lead.title.trim()) newErrors.title = "Lead title is required.";
    if (!lead.source.trim()) newErrors.source = "Please select a lead source.";
    if (
      lead.estimatedValue === "" ||
      isNaN(Number(lead.estimatedValue)) ||
      Number(lead.estimatedValue) < 0
    ) {
      newErrors.estimatedValue = "Enter a valid positive estimated value.";
    }

    setErrors((prev) => ({ ...prev, ...newErrors }));
    return (
      !newErrors.title && !newErrors.source && !newErrors.estimatedValue
    );
  };

  const handleNext = () => {
    if (step === 0 && validateStep0()) {
      setStep(1);
    }
  };

  const handleBack = () => {
    setStep(0);
  };

  const handleSubmit = async () => {
    if (!validateStep1()) return;

    setIsSubmitting(true);
    try {
    
      const payload = {
        contact: {
          firstName: lead.firstName,
          lastName: lead.lastName,
          email: lead.email,
          phone: lead.phone || null,
          jobTitle: lead.jobTitle || null,
        },
        lead: {
          title: lead.title,
          source: lead.source,
          estimatedValue: Number(lead.estimatedValue),
        },
      };

      const response = await fetch("/api/leads", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!response.ok) throw new Error("Failed to create lead");

      alert("Lead created successfully!");
    } catch (err) {
      console.error(err);
      alert("Error submitting lead. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="max-w-xl mx-auto my-10 p-6 bg-white border border-gray-200 rounded-lg shadow-sm">
      {/* Wizard Header Progress */}
      <div className="flex items-center justify-between mb-6 pb-4 border-b border-gray-100">
        <h2 className="text-xl font-semibold text-gray-800">
          {step === 0 ? "Step 1: Contact Information" : "Step 2: Lead Details"}
        </h2>
        <span className="text-sm font-medium text-gray-500">
          Step {step + 1} of 2
        </span>
      </div>

      {/* Step 0: Contact Form */}
      {step === 0 && (
        <div className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label htmlFor="firstName" className="block text-sm font-medium text-gray-700 mb-1">
                First Name *
              </label>
              <input
                type="text"
                id="firstName"
                value={lead.firstName}
                onChange={handleChange}
                className={`w-full px-3 py-2 border rounded-md outline-none text-sm ${
                  errors.firstName ? "border-red-500" : "border-gray-300 focus:border-blue-500"
                }`}
                placeholder="John"
              />
              {errors.firstName && <p className="text-xs text-red-500 mt-1">{errors.firstName}</p>}
            </div>

            <div>
              <label htmlFor="lastName" className="block text-sm font-medium text-gray-700 mb-1">
                Last Name *
              </label>
              <input
                type="text"
                id="lastName"
                value={lead.lastName}
                onChange={handleChange}
                className={`w-full px-3 py-2 border rounded-md outline-none text-sm ${
                  errors.lastName ? "border-red-500" : "border-gray-300 focus:border-blue-500"
                }`}
                placeholder="Doe"
              />
              {errors.lastName && <p className="text-xs text-red-500 mt-1">{errors.lastName}</p>}
            </div>
          </div>

          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">
              Email Address *
            </label>
            <input
              type="email"
              id="email"
              value={lead.email}
              onChange={handleChange}
              className={`w-full px-3 py-2 border rounded-md outline-none text-sm ${
                errors.email ? "border-red-500" : "border-gray-300 focus:border-blue-500"
              }`}
              placeholder="john.doe@example.com"
            />
            {errors.email && <p className="text-xs text-red-500 mt-1">{errors.email}</p>}
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label htmlFor="phone" className="block text-sm font-medium text-gray-700 mb-1">
                Phone Number
              </label>
              <input
                type="text"
                id="phone"
                value={lead.phone}
                onChange={handleChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-md outline-none text-sm focus:border-blue-500"
                placeholder="+1 555-0199"
              />
            </div>

            <div>
              <label htmlFor="jobTitle" className="block text-sm font-medium text-gray-700 mb-1">
                Job Title
              </label>
              <input
                type="text"
                id="jobTitle"
                value={lead.jobTitle}
                onChange={handleChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-md outline-none text-sm focus:border-blue-500"
                placeholder="CTO"
              />
            </div>
          </div>

          <div className="flex justify-end pt-4">
            <button
              type="button"
              onClick={handleNext}
              className="px-5 py-2 bg-blue-600 hover:bg-blue-700 text-white font-medium text-sm rounded-md transition-colors"
            >
              Next: Lead Info &rarr;
            </button>
          </div>
        </div>
      )}

      {/* Step 1: Lead Details */}
      {step === 1 && (
        <div className="space-y-4">
          <div>
            <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-1">
              Lead Opportunity Title *
            </label>
            <input
              type="text"
              id="title"
              value={lead.title}
              onChange={handleChange}
              className={`w-full px-3 py-2 border rounded-md outline-none text-sm ${
                errors.title ? "border-red-500" : "border-gray-300 focus:border-blue-500"
              }`}
              placeholder="e.g., Enterprise Cloud Migration"
            />
            {errors.title && <p className="text-xs text-red-500 mt-1">{errors.title}</p>}
          </div>

          <div>
            <label htmlFor="source" className="block text-sm font-medium text-gray-700 mb-1">
              Lead Source *
            </label>
            <select
              id="source"
              value={lead.source}
              onChange={handleChange}
              className={`w-full px-3 py-2 border rounded-md outline-none text-sm ${
                errors.source ? "border-red-500" : "border-gray-300 focus:border-blue-500"
              }`}
            >
              <option value="">-- Select Source --</option>
              <option value="Website">Website</option>
              <option value="Referral">Referral</option>
              <option value="Cold Call">Cold Call</option>
              <option value="Trade Show">Trade Show</option>
              <option value="Inbound Email">Inbound Email</option>
            </select>
            {errors.source && <p className="text-xs text-red-500 mt-1">{errors.source}</p>}
          </div>

          <div>
            <label htmlFor="estimatedValue" className="block text-sm font-medium text-gray-700 mb-1">
              Estimated Deal Value ($)
            </label>
            <input
              type="number"
              id="estimatedValue"
              step="0.01"
              value={lead.estimatedValue}
              onChange={handleChange}
              className={`w-full px-3 py-2 border rounded-md outline-none text-sm ${
                errors.estimatedValue ? "border-red-500" : "border-gray-300 focus:border-blue-500"
              }`}
              placeholder="0.00"
            />
            {errors.estimatedValue && (
              <p className="text-xs text-red-500 mt-1">{errors.estimatedValue}</p>
            )}
          </div>

          <div className="flex justify-between pt-4">
            <button
              type="button"
              onClick={handleBack}
              className="px-4 py-2 border border-gray-300 text-gray-700 hover:bg-gray-50 font-medium text-sm rounded-md transition-colors"
            >
              &larr; Back
            </button>
            <button
              type="button"
              onClick={handleSubmit}
              disabled={isSubmitting}
              className="px-5 py-2 bg-green-600 hover:bg-green-700 text-white font-medium text-sm rounded-md transition-colors disabled:opacity-50"
            >
              {isSubmitting ? "Submitting..." : "Submit Lead"}
            </button>
          </div>
        </div>
      )}
    </div>
  );
};