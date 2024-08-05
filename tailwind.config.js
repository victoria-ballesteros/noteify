/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./**/*.{razor,cshtml,html}'],
  theme: {
    extend: {
      colors: {
        'spotify-green': '#1db954',
        'spotify-dark-grey': '#121212',
        'spotify-grey': '#212121',
        'spotify-light-grey': '#535353',
        'spotify-white': '#b3b3b3',
        'spotify-black': '#000000',
        'spotify-pink': '#ed71ad',
        'spotify-purple': '#7277f1',
        'spotify-electric-purple': '#4300ff',
        'spotify-darkest-gradient': '#717371'
      },
    },
  },
  plugins: [],
};
