import "../styles/globals.css";
import type { AppProps } from "next/app";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { Toaster } from "react-hot-toast";

const queryClient = new QueryClient({
  defaultOptions: {
    mutations: {
      onError: (e, v, c) => {
        console.log(e, v, c);
      },
    },
    queries: {
      staleTime: Infinity,
    },
  },
});
function MyApp({ Component, pageProps }: AppProps) {
  return (
    <>
      <QueryClientProvider client={queryClient}>
        <ReactQueryDevtools initialIsOpen={false} />
        <Component {...pageProps} />
        <Toaster />
      </QueryClientProvider>
    </>
  );
}

export default MyApp;
