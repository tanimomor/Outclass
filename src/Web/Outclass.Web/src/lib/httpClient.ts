import axios from "axios";

// Base URL for the Gateway
const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api";

const api = axios.create({
    baseURL: API_URL,
    headers: {
        "Content-Type": "application/json",
    },
    withCredentials: true,
});

api.interceptors.request.use((config) => {
    if (typeof window !== "undefined") {
        const token = localStorage.getItem("accessToken");
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }

        const tenantId = localStorage.getItem("tenantId");
        if (tenantId) {
            config.headers["X-Tenant-Id"] = tenantId;
        }
    }
    return config;
});

api.interceptors.response.use(
    (response) => response,
    async (error) => {
        // Handle 401 Unauthorized globally if needed (e.g., redirect to login)
        if (error.response?.status === 401) {
            if (typeof window !== "undefined") {
                // window.location.href = "/login";
            }
        }
        return Promise.reject(error);
    }
);

export default api;
