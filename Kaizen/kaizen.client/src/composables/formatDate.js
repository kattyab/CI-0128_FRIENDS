export function formatDate(date) {
  if (!date) {
    return "";
  }
  return new Date(date).toLocaleDateString("es-CR", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  });
}
