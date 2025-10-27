import { createContext, useContext, useState } from "react";

const AuthContext = createContext();

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
  const [role, setRole] = useState(localStorage.getItem("role") || "Customer");

  const login = (userRole) => {
    localStorage.setItem("role", userRole);
    setRole(userRole);
  };

  const logout = () => {
    localStorage.removeItem("role");
    setRole("");
  };
  return (
    <AuthContext.Provider value={{ login, role, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
