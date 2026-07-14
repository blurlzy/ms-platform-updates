import { ErrorHandler, Injectable, signal } from '@angular/core';


@Injectable({ providedIn: 'root' })
export class GlobalErrorHandler implements ErrorHandler {
  readonly message = signal<string | null>(null);

  handleError(error: unknown): void {
    this.message.set('An unexpected error occurred.');

    // preserve the original error in the console for debugging
    console.error(error);
  }

  dismiss(): void {
    this.message.set(null);
  }

  private extractMessage(error: unknown): string {
    if (error instanceof Error) {
      return error.message || 'An unexpected error occurred.';
    }
    if (typeof error === 'string') {
      return error;
    }

    if (this.hasMessage(error)) {
      return error.message;
    }

    return 'An unexpected error occurred.';
  }

  private hasMessage(error: unknown): error is { message: string } {
    return typeof error === 'object'
      && error !== null
      && 'message' in error
      && typeof error.message === 'string'
      && error.message.length > 0;
  }
}
