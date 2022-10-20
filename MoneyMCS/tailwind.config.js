/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["./**/*.cshtml", "./node_modules/flowbite/**/*.js"],
  theme: {
      extend: {
          colors: {
              brandcolor1: '#49A247',
              brandcolor2: '#A0CC3A',
              brandcolor3: '#0F1A23',
              brandcolor4: '#81B33F',
          }
      },
  },
    plugins: [
        require('flowbite/plugin')
    ],
}
