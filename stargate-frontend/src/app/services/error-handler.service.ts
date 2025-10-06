import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

export interface ErrorInfo {
  message: string;
  code?: string;
  details?: any;
  timestamp: Date;
}

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  
  handleError(error: any): ErrorInfo {
    let errorInfo: ErrorInfo = {
      message: 'An unexpected error occurred',
      timestamp: new Date()
    };

    if (error instanceof HttpErrorResponse) {
      errorInfo = this.handleHttpError(error);
    } else if (error instanceof Error) {
      errorInfo.message = error.message;
    } else if (typeof error === 'string') {
      errorInfo.message = error;
    }

    // Log error for debugging
    console.error('Error handled:', errorInfo);
    
    return errorInfo;
  }

  private handleHttpError(error: HttpErrorResponse): ErrorInfo {
    const errorInfo: ErrorInfo = {
      message: 'A network error occurred',
      code: error.status.toString(),
      timestamp: new Date()
    };

    switch (error.status) {
      case 0:
        errorInfo.message = 'Unable to connect to the server. Please check your connection.';
        break;
      case 400:
        errorInfo.message = error.error?.message || 'Invalid request. Please check your input.';
        break;
      case 401:
        errorInfo.message = 'You are not authorized to perform this action.';
        break;
      case 403:
        errorInfo.message = 'Access denied. You do not have permission to perform this action.';
        break;
      case 404:
        errorInfo.message = 'The requested resource was not found.';
        break;
      case 409:
        errorInfo.message = error.error?.message || 'A conflict occurred. The resource may already exist.';
        break;
      case 422:
        errorInfo.message = error.error?.message || 'Validation failed. Please check your input.';
        break;
      case 500:
        errorInfo.message = 'A server error occurred. Please try again later.';
        break;
      case 502:
        errorInfo.message = 'Bad gateway. The server is temporarily unavailable.';
        break;
      case 503:
        errorInfo.message = 'Service unavailable. Please try again later.';
        break;
      default:
        errorInfo.message = error.error?.message || `An error occurred (${error.status}). Please try again.`;
    }

    return errorInfo;
  }

  getErrorMessage(error: any): string {
    return this.handleError(error).message;
  }
}
