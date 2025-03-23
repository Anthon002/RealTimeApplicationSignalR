module.exports = {
  mode: 'jit',
  content: ["./Views/**/*.cshtml", "./Pages/**/*.cshtml", "./wwwroot/**/*.html"],
  purge: [],
  darkMode: false, // or 'media' or 'class'
  theme: {
    extend: {},
  },
  variants: {
    extend: {},
  },
  plugins: [],
}
