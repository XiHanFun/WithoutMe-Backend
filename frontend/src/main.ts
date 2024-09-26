/**
 * 主入口文件
 */

import { createApp } from "vue";
import App from "./App.vue";
import { setupLibraries } from "./app/setupLibrary";
import { setupPlugins } from "./app/setupPlugin";

const app = createApp(App);
await setupLibraries(app);
await setupPlugins(app);
app.mount("#app");
