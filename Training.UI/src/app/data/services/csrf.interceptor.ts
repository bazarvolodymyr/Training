import { HttpInterceptorFn } from '@angular/common/http';

function readCookie(name: string): string | null {
  const prefix = `${name}=`
  for (const part of document.cookie.split(';')) {
    const trimmed = part.trim()
    if (trimmed.startsWith(prefix)) {
      return decodeURIComponent(trimmed.slice(prefix.length))
    }
  }
  return null
}

export const csrfInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.method === 'GET' || req.method === 'HEAD') {
    return next(req)
  }

  const token = readCookie('XSRF-TOKEN')
  if (!token) {
    return next(req)
  }

  return next(req.clone({
    headers: req.headers.set('X-XSRF-TOKEN', token)
  }))
}
