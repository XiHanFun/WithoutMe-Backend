/**
 * 入口
 */

import { createSSRApp } from 'vue';
import { RouterHistory } from 'vue-router';
import { Language, LanguageKey, languages } from '@/language';
import { createUniversalStore } from '@/store';
import { createI18n } from '/@/composables/i18n';
import { createHead, Head } from '@/composables/head';
import { createTheme, Theme } from '@/composables/theme';
import { createUniversalRouter, RouterCreatorOptions } from '@/app/router';
import { createGlobalState, LayoutColumn, RenderErrorValue } from '@/app/state';
// import components from '@/app/components';
import App from '@/app/App.vue';

interface ICreatorContext {
  historyCreator(base?: string): RouterHistory;
  routerBeforeMiddleware?(globalState: any): RouterCreatorOptions['beforeMiddleware'];
  routerAfterMiddleware?(globalState: any): RouterCreatorOptions['afterMiddleware'];
  layout?: LayoutColumn;
  theme: Theme;
  region: string;
  language: string;
  userAgent: string;
  error?: RenderErrorValue;
}

type MainApp = ReturnType<typeof createMainApp>;
const createMainApp = (context: ICreatorContext) => {
  // 1. create app
  const app = createSSRApp(App);
  // 2. global state
  const globalState = createGlobalState({
    userAgent: context.userAgent || '',
    language: context.language || '',
    layout: context.layout ?? LayoutColumn.Normal,
    error: context.error,
  });
  // 3. store
  const store = createUniversalStore({ globalState });
  // 4. router
  const router = createUniversalRouter({
    beforeMiddleware: context.routerBeforeMiddleware?.(globalState),
    afterMiddleware: context.routerAfterMiddleware?.(globalState),
    history: context.historyCreator(),
  });
  // 5. composables
  const head = createHead();
  const theme = createTheme(context.theme);
  const i18n = createI18n({
    default: globalState.userAgent.isZhUser ? Language.Chinese : Language.English,
    keys: Object.values(LanguageKey),
    languages,
  });

  // global head attrbutes: for use in different environments
  const getGlobalHead = (): Head => ({
    htmlAttrs: {
      lang: i18n.l.value?.iso ?? '',
      'data-region': context.region,
      'data-theme': theme.theme.value,
      'data-device': globalState.userAgent.isMobile ? 'mobile' : 'desktop',
    },
  });

  // handle global error
  app.config.errorHandler = error => globalState.setRenderError(error);
  // handle router error https://next.router.vuejs.org/api/#onerror
  router.onError(globalState.setRenderError);

  // handle router validate error & 404 error
  // https://next.router.vuejs.org/guide/advanced/navigation-guards.html#optional-third-argument-next
  router.beforeEach((to, _, next) => {
    if (to.meta.validator) {
      to.meta
        .validator({ route: to, i18n, store })
        .then(next)
        .catch(error => {
          // next(error) > router error > global state error
          const newError: any = new Error();
          newError.code = error.code;
          newError.message = error.message;
          next(newError);
        });
    } else {
      next();
    }
  });

  app.use(router);
  app.use(store);
  app.use(globalState);
  app.use(i18n);
  app.use(head);
  app.use(theme);
  // app.use(components);

  // directives
  //   app.directive(disabledWallflowerDirectiveName, vDisabledWallflower);

  return {
    app,
    router,
    store,
    globalState,
    i18n,
    head,
    theme,
    getGlobalHead,
  };
};

export { ICreatorContext, MainApp, createMainApp };
