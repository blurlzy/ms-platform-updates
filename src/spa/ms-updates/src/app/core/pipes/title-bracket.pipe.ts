import { Pipe, PipeTransform } from '@angular/core';

type TitleSource = { title?: string | null } | string | null | undefined;

@Pipe({
  name: 'titleBracket',
})
export class TitleBracketPipe implements PipeTransform {
  transform(value: TitleSource): string {
    const title = typeof value === 'string' ? value : value?.title;

    if (!title) {
      return '';
    }

    const openingBracketIndex = title.indexOf('[');
    const closingBracketIndex = title.indexOf(']', openingBracketIndex + 1);

    if (openingBracketIndex === -1 || closingBracketIndex === -1) {
      return '';
    }

    return title.slice(openingBracketIndex + 1, closingBracketIndex).trim();
  }
}
