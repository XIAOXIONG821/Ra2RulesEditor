const withPlugins = require("next-compose-plugins");
const withTM = require("next-transpile-modules")(["antd", "@ant-design/icons"]);

/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  distDir: "dist",
};

const plugins = [[withTM]];

module.exports = withPlugins(plugins, nextConfig);
