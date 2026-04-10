const Spinner = ({ message = "Loading..." }) => {
  return (
    <div className="flex flex-col items-center justify-center min-h-[calc(100vh-98px)] gap-3">
      <div
        className="animate-spin rounded-full"
        style={{
          width: 36,
          height: 36,
          border: "3px solid #314568",
          borderTop: "3px solid #0950C3",
        }}
      />
      <span className="text-[#7D7F82] text-sm">{message}</span>
    </div>
  );
};

export default Spinner;
