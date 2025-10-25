"use client"

import { createContext, useContext, useState, type ReactNode } from "react"

type Language = "uz" | "ru"

interface LanguageContextType {
  language: Language
  setLanguage: (lang: Language) => void
  t: (key: string) => string
}

const translations = {
  uz: {
    "nav.home": "Bosh sahifa",
    "nav.movies": "Filmlar",
    "nav.tvShows": "TV Seriallar",
    "nav.genres": "Janrlar",
    "nav.search": "Qidirish...",
    "nav.profile": "Profil",
    "nav.favorites": "Sevimlilar",
    "nav.history": "Tarix",
    "nav.admin": "Admin panel",
    "nav.login": "Kirish",
    "nav.register": "Ro'yxatdan o'tish",
    "nav.logout": "Chiqish",
    "login.title": "Tizimga kirish",
    "login.email": "Email",
    "login.password": "Parol",
    "login.signIn": "Kirish",
    "login.noAccount": "Hisobingiz yo'qmi?",
    "login.signUp": "Ro'yxatdan o'ting!",
    "login.success": "Muvaffaqiyat",
    "login.welcomeBack": "Xush kelibsiz!",
    "login.error": "Xatolik",
    "login.invalidCredentials": "Email yoki parol noto'g'ri",
    "register.title": "Ro'yxatdan o'tish",
    "register.name": "Ism",
    "register.email": "Email",
    "register.password": "Parol",
    "register.confirmPassword": "Parolni tasdiqlang",
    "register.signUp": "Ro'yxatdan o'tish",
    "register.hasAccount": "Hisobingiz bormi?",
    "register.signIn": "Kiring!",
    "register.success": "Muvaffaqiyat",
    "register.accountCreated": "Hisob yaratildi",
    "register.error": "Xatolik",
    "register.passwordMismatch": "Parollar mos kelmaydi",
    "register.failed": "Ro'yxatdan o'tishda xatolik",
    "common.loading": "Yuklanmoqda...",
  },
  ru: {
    "nav.home": "Главная",
    "nav.movies": "Фильмы",
    "nav.tvShows": "ТВ Сериалы",
    "nav.genres": "Жанры",
    "nav.search": "Поиск...",
    "nav.profile": "Профиль",
    "nav.favorites": "Избранное",
    "nav.history": "История",
    "nav.admin": "Админ панель",
    "nav.login": "Войти",
    "nav.register": "Регистрация",
    "nav.logout": "Выйти",
    "login.title": "Вход в систему",
    "login.email": "Email",
    "login.password": "Пароль",
    "login.signIn": "Войти",
    "login.noAccount": "Нет аккаунта?",
    "login.signUp": "Зарегистрируйтесь!",
    "login.success": "Успех",
    "login.welcomeBack": "Добро пожаловать!",
    "login.error": "Ошибка",
    "login.invalidCredentials": "Неверный email или пароль",
    "register.title": "Регистрация",
    "register.name": "Имя",
    "register.email": "Email",
    "register.password": "Пароль",
    "register.confirmPassword": "Подтвердите пароль",
    "register.signUp": "Зарегистрироваться",
    "register.hasAccount": "Есть аккаунт?",
    "register.signIn": "Войдите!",
    "register.success": "Успех",
    "register.accountCreated": "Аккаунт создан",
    "register.error": "Ошибка",
    "register.passwordMismatch": "Пароли не совпадают",
    "register.failed": "Ошибка регистрации",
    "common.loading": "Загрузка...",
  },
}

const LanguageContext = createContext<LanguageContextType | undefined>(undefined)

export function LanguageProvider({ children }: { children: ReactNode }) {
  const [language, setLanguage] = useState<Language>("uz")

  const t = (key: string): string => {
    return translations[language][key as keyof (typeof translations)[Language]] || key
  }

  return <LanguageContext.Provider value={{ language, setLanguage, t }}>{children}</LanguageContext.Provider>
}

export function useLanguage() {
  const context = useContext(LanguageContext)
  if (context === undefined) {
    throw new Error("useLanguage must be used within a LanguageProvider")
  }
  return context
}
