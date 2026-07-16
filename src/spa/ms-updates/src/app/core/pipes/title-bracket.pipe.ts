import { Pipe, PipeTransform } from '@angular/core';

type TitleSource = { title?: string | null } | string | null | undefined;

@Pipe({
  name: 'titleBracket',
})
export class TitleBracketPipe implements PipeTransform {
  transform(value: TitleSource): boolean {
    const title = typeof value === 'string' ? value : value?.title;

    return title?.toLocaleLowerCase().includes('retirement') ?? false;
  }
}
