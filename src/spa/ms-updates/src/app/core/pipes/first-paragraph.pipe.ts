import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
	name: 'firstParagraph',
})
export class FirstParagraphPipe implements PipeTransform {
	transform(value: string | null | undefined): string {
		if (!value) {
			return '';
		}

		const paragraphMatch = value.match(/<p\b[^>]*>([\s\S]*?)<\/p\s*>/i);

		if (paragraphMatch) {
			return this.getTextContent(paragraphMatch[1]);
		}

		return this.getFirstSentence(this.getTextContent(value));
	}

	private getTextContent(value: string): string {
		const container = document.createElement('div');
		container.innerHTML = value;

		return container.textContent?.trim() ?? '';
	}

	private getFirstSentence(value: string): string {
		const sentence = value.match(/^.*?[.!?](?=\s|$)/);

		return (sentence?.[0] ?? value).trim();
	}
}
